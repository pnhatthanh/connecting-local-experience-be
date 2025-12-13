using Booking.Domain.Enums;
using FluentValidation;

namespace Booking.Application.Handlers.Commands.ToggleBookingStatus
{
    public class ToggleBookingStatusCommandValidator : AbstractValidator<ToggleBookingStatusCommand>
    {
        public ToggleBookingStatusCommandValidator()
        {
            RuleFor(x => x.BookingId)
                .NotEmpty().WithMessage("BookingId is required");
            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .Must(status => Enum.TryParse<BookingStatus>(status, true ,out _))
                .WithMessage("Invalid status value");
            
        }
    }
}   