using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Domain.Interfaces;
using Booking.Application.Events;
using Booking.Application.Interfaces;
using Booking.Domain.Enums;
using Booking.Domain.Repositories;
using Booking.Domain.Specifications;
using Microsoft.Extensions.Logging;
using BuildingBlocks.Application.EventBus.Abstractions;

namespace Booking.Application.Handlers.Commands.ProcessVnPayCallback
{
    public class ProcessVnPayCallbackCommandHandler : ICommandHandler<ProcessVnPayCallbackCommand, ProcessVnPayCallbackResponse>
    {
        private readonly IVnPayService _vnPayService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventBus _eventBus;
        private readonly ILogger<ProcessVnPayCallbackCommandHandler> _logger;

        public ProcessVnPayCallbackCommandHandler(IVnPayService vnPayService, IPaymentRepository paymentRepository,
            IBookingRepository bookingRepository, IUnitOfWork unitOfWork, IEventBus eventBus,
            ILogger<ProcessVnPayCallbackCommandHandler> logger)
        {
            _vnPayService = vnPayService;
            _paymentRepository = paymentRepository;
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task<ProcessVnPayCallbackResponse> Handle(ProcessVnPayCallbackCommand request, CancellationToken cancellationToken)
        {
            var (isValid, message, transactionId) = await _vnPayService.ValidateCallbackAsync(request.QueryParams);

            if (!isValid)
            {
                _logger.LogWarning("VNPay callback validation failed: {Message}", message);
                return new ProcessVnPayCallbackResponse(false, message);
            }
            var vnpTxnRef = request.QueryParams?["vnp_TxnRef"].ToString() ?? "";
            _logger.LogInformation("Processing VNPay callback for Transaction: {TxnRef}", vnpTxnRef);
            var paymentSpec = new PaymentByTransactionIdSpecification(vnpTxnRef);
            var payment = await _paymentRepository.GetBySpecAsync(paymentSpec);
            if (payment == null)
            {
                _logger.LogWarning("Payment not found for VNPay callback: {TxnRef}", vnpTxnRef);
                return new ProcessVnPayCallbackResponse(false, "Payment not found");
            }
            var booking = await _bookingRepository.GetByIdAsync(payment.BookingId);
            if (booking == null)
            {
                _logger.LogWarning("Booking not found for VNPay callback: {BookingId}", payment.BookingId);
                return new ProcessVnPayCallbackResponse(false, "Booking not found");
            }
            payment.Status = PaymentStatus.Paid;
            payment.PaidAt = DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;
            _paymentRepository.Update(payment);

            booking.Status = BookingStatus.Confirmed;
            booking.UpdatedAt = DateTime.UtcNow;
            _bookingRepository.Update(booking);
            
            await _unitOfWork.SaveChangeAsync();

            var bookingConfirmedEvent = new BookingConfirmedEvent(
                booking.Id,
                booking.ExperienceId,
                booking.Date,
                booking.StartTime,
                booking.EndTime,
                booking.Adults,
                booking.Children
            );
            await _eventBus.PublishAsync(bookingConfirmedEvent);
            
            _logger.LogInformation("VNPay payment processed successfully for booking {BookingId}, Transaction: {TransId}. Event published to update slot availability.",
                payment.BookingId, transactionId);
            return new ProcessVnPayCallbackResponse(true, "Payment successful", booking.BookingCode);
        }
    }
}
