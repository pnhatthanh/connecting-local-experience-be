using FluentValidation;

namespace Booking.Application.Handlers.Commands.CancelBooking
{
    public class CancelBookingCommandValidator : AbstractValidator<CancelBookingCommand>
    {
        public CancelBookingCommandValidator()
        {
            RuleFor(x => x.BookingId)
                .NotEmpty().WithMessage("Booking ID is required");
            
            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Cancellation reason is required")
                .MaximumLength(1000).WithMessage("Reason cannot exceed 1000 characters");
        }
    }
}
