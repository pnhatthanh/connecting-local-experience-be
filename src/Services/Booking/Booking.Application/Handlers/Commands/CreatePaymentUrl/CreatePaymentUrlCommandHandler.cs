using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using Booking.Application.Interfaces;
using Booking.Domain.Enums;
using Booking.Domain.Repositories;
using Booking.Domain.Specifications;
using Microsoft.Extensions.Logging;

namespace Booking.Application.Handlers.Commands.CreatePaymentUrl
{
    public class CreatePaymentUrlCommandHandler : ICommandHandler<CreatePaymentUrlCommand, CreatePaymentUrlResponse>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMomoService _momoService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreatePaymentUrlCommandHandler> _logger;

        public CreatePaymentUrlCommandHandler(
            IBookingRepository bookingRepository,
            IPaymentRepository paymentRepository,
            IMomoService momoService,
            IUnitOfWork unitOfWork,
            ILogger<CreatePaymentUrlCommandHandler> logger)
        {
            _bookingRepository = bookingRepository;
            _paymentRepository = paymentRepository;
            _momoService = momoService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CreatePaymentUrlResponse> Handle(CreatePaymentUrlCommand request, CancellationToken cancellationToken)
        {
            var bookingSpec = new BookingByIdSpecification(request.BookingId);
            var booking = await _bookingRepository.GetBySpecAsync(bookingSpec);
            if (booking == null)
                throw new NotFoundException($"Booking with ID {request.BookingId} not found");

            var paymentSpec = new PaymentByBookingIdSpecification(request.BookingId);
            var payment = await _paymentRepository.GetBySpecAsync(paymentSpec);
            if (payment == null)
                throw new NotFoundException("Payment not found for booking");

            if (payment.Status == PaymentStatus.Paid)
                throw new BadRequestException("Booking already paid");

            var momoResult = await _momoService.CreatePaymentUrlAsync(
                request.BookingId,
                payment.Amount,
                booking.BookingCode,
                request.IpAddress);

            if (!momoResult.success)
            {
                _logger.LogWarning("Failed to create Momo payment URL for booking {BookingId}: {Message}",
                    request.BookingId, momoResult.message);
                return new CreatePaymentUrlResponse(false, string.Empty, "Momo", momoResult.message);
            }

            await _unitOfWork.SaveChangeAsync();

            _logger.LogInformation("Payment URL created successfully for booking {BookingId} using Momo",
                request.BookingId);

            return new CreatePaymentUrlResponse(true, momoResult.paymentUrl, "Momo", "Payment URL created successfully");
        }
    }
}
