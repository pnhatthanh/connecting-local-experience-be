using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Application.EventBus.Abstractions;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using Booking.Application.Events;
using Booking.Application.Interfaces;
using Booking.Domain.Entities;
using Booking.Domain.Enums;
using Booking.Domain.Repositories;
using Booking.Domain.Specifications;
using Microsoft.Extensions.Logging;

namespace Booking.Application.Handlers.Commands.CancelBooking
{
    public class CancelBookingCommandHandler : ICommandHandler<CancelBookingCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IRefundRepository _refundRepository;
        private readonly IBookingCancellationRepository _cancellationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExperienceService _experienceService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEventBus _eventBus;
        private readonly ILogger<CancelBookingCommandHandler> _logger;

        public CancelBookingCommandHandler(
            IBookingRepository bookingRepository,
            IPaymentRepository paymentRepository,
            IRefundRepository refundRepository,
            IBookingCancellationRepository cancellationRepository,
            IUnitOfWork unitOfWork,
            IExperienceService experienceService,
            ICurrentUserService currentUserService,
            IEventBus eventBus,
            ILogger<CancelBookingCommandHandler> logger)
        {
            _bookingRepository = bookingRepository;
            _paymentRepository = paymentRepository;
            _refundRepository = refundRepository;
            _cancellationRepository = cancellationRepository;
            _unitOfWork = unitOfWork;
            _experienceService = experienceService;
            _currentUserService = currentUserService;
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task<bool> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            // Get booking with related entities using specification
            var spec = new BookingByIdSpecification(request.BookingId);
            var booking = await _bookingRepository.GetBySpecAsync(spec, b => b.Payment!, b => b.Cancellation!);
            if (booking == null)
                throw new NotFoundException("Booking not found");

            // Validate cancellation permission
            if (!request.IsCancelledByHost && booking.UserId != userId)
                throw new ForbiddenException("You don't have permission to cancel this booking");

            if (request.IsCancelledByHost && booking.HostId != userId)
                throw new ForbiddenException("You don't have permission to cancel this booking");

            // Check if booking can be cancelled
            if (booking.Status == BookingStatus.Cancelled)
                throw new BadRequestException("Booking is already cancelled");

            if (booking.Status == BookingStatus.Completed)
                throw new BadRequestException("Cannot cancel a completed booking");

            // Get experience for cancellation policy
            var experience = await _experienceService.GetExperienceAsync(booking.ExperienceId);
            if (experience == null)
                throw new NotFoundException("Experience not found");

            // Get payment using specification
            var paymentSpec = new PaymentByBookingIdSpecification(booking.Id);
            var payment = await _paymentRepository.GetBySpecAsync(paymentSpec);
            if (payment == null || payment.Status != PaymentStatus.Paid)
            {
                // If not paid yet, just cancel without refund
                booking.Status = BookingStatus.Cancelled;
                booking.UpdatedAt = DateTime.UtcNow;

                var cancellation = new BookingCancellationEntity
                {
                    Id = Guid.NewGuid(),
                    BookingId = booking.Id,
                    CancelledBy = request.IsCancelledByHost ? CancelledBy.Host : CancelledBy.Traveler,
                    Reason = request.Reason,
                    CancellationFee = 0,
                    RefundAmount = 0,
                    CancelledAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                await _cancellationRepository.AddAsync(cancellation);
                await _unitOfWork.SaveChangeAsync();

                await PublishCancellationEvent(booking, cancellation);
                return true;
            }

            // Calculate refund based on cancellation policy
            var bookingStartDateTime = booking.Date.ToDateTime(TimeOnly.FromTimeSpan(booking.StartTime));
            var (refundAmount, cancellationFee) = CalculateRefund(
                booking.TotalPrice, 
                experience.CancellationPolicy, 
                bookingStartDateTime,
                request.IsCancelledByHost
            );

            // Create cancellation record
            var cancellationEntity = new BookingCancellationEntity
            {
                Id = Guid.NewGuid(),
                BookingId = booking.Id,
                CancelledBy = request.IsCancelledByHost ? CancelledBy.Host : CancelledBy.Traveler,
                Reason = request.Reason,
                CancellationFee = cancellationFee,
                RefundAmount = refundAmount,
                CancelledAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            // Process refund if applicable
            if (refundAmount > 0)
            {
                var refund = new RefundEntity
                {
                    Id = Guid.NewGuid(),
                    PaymentId = payment.Id,
                    BookingId = booking.Id,
                    RefundAmount = refundAmount,
                    Currency = "VND",
                    Reason = request.Reason,
                    Status = RefundStatus.Requested,
                    RequestedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                // Refund will be processed manually or via VNPay
                refund.Status = RefundStatus.Requested;
                _logger.LogInformation("Refund requested for booking {BookingId}, Amount: {Amount}", 
                    booking.Id, refundAmount);

                await _refundRepository.AddAsync(refund);
            }

            // Update booking status
            booking.Status = BookingStatus.Cancelled;
            booking.UpdatedAt = DateTime.UtcNow;

            await _cancellationRepository.AddAsync(cancellationEntity);
            await _unitOfWork.SaveChangeAsync();

            // Publish cancellation event to release slots
            await PublishCancellationEvent(booking, cancellationEntity);

            return true;
        }

        private (decimal refundAmount, decimal cancellationFee) CalculateRefund(
            decimal totalPrice, 
            string cancellationPolicy, 
            DateTime bookingStartTime,
            bool isCancelledByHost)
        {
            if (isCancelledByHost)
                return (totalPrice, 0);

            var hoursUntilStart = (bookingStartTime - DateTime.UtcNow).TotalHours;

            return cancellationPolicy switch
            {
                "AlwaysFreeCancellation" => (totalPrice, 0),
                
                "FreeCancellation24Hours" when hoursUntilStart >= 24 => (totalPrice, 0),
                "FreeCancellation24Hours" when hoursUntilStart < 24 => (totalPrice * 0.5m, totalPrice * 0.5m),
                
                "NonRefundable" => (0, totalPrice),
                
                _ => (totalPrice, 0) 
            };
        }

        private async Task PublishCancellationEvent(BookingEntity booking, BookingCancellationEntity cancellation)
        {
            var startDateTime = booking.Date.ToDateTime(TimeOnly.FromTimeSpan(booking.StartTime));
            var endDateTime = booking.Date.ToDateTime(TimeOnly.FromTimeSpan(booking.EndTime));
            
            var cancellationEvent = new BookingCancelledEvent(
                booking.Id,
                booking.ExperienceId,
                booking.BookingCode,
                startDateTime,
                endDateTime,
                booking.Adults,
                booking.Children,
                cancellation.CancelledBy.ToString(),
                cancellation.Reason,
                cancellation.RefundAmount
            );

            await _eventBus.PublishAsync(cancellationEvent);
        }
    }
}
