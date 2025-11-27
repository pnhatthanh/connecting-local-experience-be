using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Domain.Interfaces;
using Booking.Application.Interfaces;
using Booking.Domain.Enums;
using Booking.Domain.Repositories;
using Booking.Domain.Specifications;
using Microsoft.Extensions.Logging;

namespace Booking.Application.Handlers.Commands.ProcessPaymentCallback
{
    public class ProcessMomoCallbackCommandHandler : ICommandHandler<ProcessMomoCallbackCommand, ProcessPaymentCallbackResponse>
    {
        private readonly IMomoService _momoService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProcessMomoCallbackCommandHandler> _logger;
        public ProcessMomoCallbackCommandHandler(
            IMomoService momoService,
            IPaymentRepository paymentRepository,
            IBookingRepository bookingRepository,
            IUnitOfWork unitOfWork,
            ILogger<ProcessMomoCallbackCommandHandler> logger)
        {
            _momoService = momoService;
            _paymentRepository = paymentRepository;
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ProcessPaymentCallbackResponse> Handle(ProcessMomoCallbackCommand request, CancellationToken cancellationToken)
        {
            var (isValid, message, transactionId) = await _momoService.ValidateCallbackAsync(request.QueryParams);

            if (!isValid)
            {
                _logger.LogWarning("Momo callback validation failed: {Message}", message);
                return new ProcessPaymentCallbackResponse(false, message);
            }

            var orderId = request.QueryParams.GetValueOrDefault("orderId", "");
            if (!Guid.TryParse(orderId, out var bookingId))
            {
                _logger.LogWarning("Invalid booking ID in Momo callback: {OrderId}", orderId);
                return new ProcessPaymentCallbackResponse(false, "Invalid order ID");
            }

            var bookingSpec = new BookingByIdSpecification(bookingId);
            var booking = await _bookingRepository.GetBySpecAsync(bookingSpec);
            if (booking == null)
            {
                _logger.LogWarning("Booking not found for Momo callback: {BookingId}", bookingId);
                return new ProcessPaymentCallbackResponse(false, "Booking not found");
            }

            var paymentSpec = new PaymentByBookingIdSpecification(bookingId);
            var payment = await _paymentRepository.GetBySpecAsync(paymentSpec);
            if (payment == null)
            {
                _logger.LogWarning("Payment not found for Momo callback: {BookingId}", bookingId);
                return new ProcessPaymentCallbackResponse(false, "Payment not found");
            }

            // Update payment status
            payment.Status = PaymentStatus.Paid;
            payment.TransactionId = transactionId;
            payment.PaidAt = DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;
            _paymentRepository.Update(payment);

            // Update booking status
            booking.Status = BookingStatus.Confirmed;
            booking.UpdatedAt = DateTime.UtcNow;
            _bookingRepository.Update(booking);

            await _unitOfWork.SaveChangeAsync();

            _logger.LogInformation("Momo payment processed successfully for booking {BookingId}, Transaction: {TransId}",
                bookingId, transactionId);

            return new ProcessPaymentCallbackResponse(true, "Payment successful", booking.BookingCode);
        }
    }
}
