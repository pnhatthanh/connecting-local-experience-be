using FluentValidation;

namespace Booking.Application.Handlers.Commands.CreateBooking
{
    public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
    {
        public CreateBookingCommandValidator()
        {
            RuleFor(x => x.ExperienceId)
                .NotEmpty().WithMessage("Experience ID is required");
            
            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required")
                .Must(date => date >= DateOnly.FromDateTime(DateTime.UtcNow.Date))
                .WithMessage("Date must be today or in the future");
            
            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Start time is required");
            
            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("End time is required")
                .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time");
            
            RuleFor(x => x.Adults)
                .GreaterThan(0).WithMessage("At least one adult is required");
            
            RuleFor(x => x.Children)
                .GreaterThanOrEqualTo(0).WithMessage("Children count cannot be negative");
            
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(255).WithMessage("First name cannot exceed 255 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(255).WithMessage("Last name cannot exceed 255 characters");
            
            RuleFor(x => x.ContactEmail)
                .NotEmpty().WithMessage("Contact email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(255).WithMessage("Contact email cannot exceed 255 characters");
            
            RuleFor(x => x.ContactPhone)
                .NotEmpty().WithMessage("Contact phone is required")
                .MaximumLength(50).WithMessage("Contact phone cannot exceed 50 characters");
        }
    }
}
