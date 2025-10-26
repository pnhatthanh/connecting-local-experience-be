using Experience.Domain.Enums;
using FluentValidation;

namespace Experience.Application.Handlers.Commands.UpdateExperience
{
    public class UpdateExperienceCommandValidator : AbstractValidator<UpdateExperienceCommand>
    {
        public UpdateExperienceCommandValidator()
        {
            RuleFor(x => x.ExperienceId)
                .NotEmpty()
                .WithMessage("Experience ID is required.");
            
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MaximumLength(255)
                .WithMessage("Title must not exceed 255 characters.");
            
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required.");
            
            RuleFor(x => x.Address)
                .NotEmpty()
                .WithMessage("Address is required.")
                .MaximumLength(500)
                .WithMessage("Address must not exceed 500 characters.");
            
            RuleFor(x => x.District)
                .NotEmpty()
                .WithMessage("District is required.")
                .MaximumLength(100)
                .WithMessage("District must not exceed 100 characters.");
            
            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("City is required.")
                .MaximumLength(100)
                .WithMessage("City must not exceed 100 characters.");
            
            RuleFor(x => x.Country)
                .NotEmpty()
                .WithMessage("Country is required.")
                .MaximumLength(100)
                .WithMessage("Country must not exceed 100 characters.");
            
            RuleFor(x => x.AdultPrice)
                .GreaterThan(0)
                .WithMessage("Adult price must be greater than 0.");
                
            RuleFor(x => x.ChildPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Child price must be greater than or equal to 0.")
                .Must((command, childPrice) => 
                {
                    if (command.MinAge < 12)
                        return childPrice > 0;
                    else
                        return childPrice == 0;
                })
                .WithMessage("Child price must be greater than 0 if minimum age is below 12, or equal to 0 if minimum age is 12 or above.");
                
            RuleFor(x => x.Duration)
                .GreaterThan(0)
                .WithMessage("Duration must be greater than 0.");
            
            RuleFor(x => x.MaxParticipants)
                .GreaterThan(0)
                .WithMessage("Max participants must be greater than 0.");
            
            RuleFor(x => x.CategoryId)
                .NotEmpty()
                .WithMessage("Category is required.");
            
            RuleFor(x => x.MinAge)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Minimum age must be greater than or equal to 0.");
            
            RuleFor(x => x.CancellationPolicy)
                .IsInEnum()
                .WithMessage("Cancellation policy must be a valid type.");

            RuleFor(x => x.MeetingPoint)
                .NotNull()
                .WithMessage("Meeting point is required.");
            
            RuleFor(x => x.MeetingLocation)
                .NotEmpty()
                .WithMessage("Meeting location is required.")
                .MaximumLength(500)
                .WithMessage("Meeting location must not exceed 500 characters.");

            // Schedule validation
            RuleFor(x => x.RecurrenceType)
                .IsInEnum()
                .WithMessage("Invalid recurrence type.");

            RuleFor(x => x.TimeSlots)
                .NotEmpty()
                .WithMessage("At least one time slot is required.");

            RuleForEach(x => x.TimeSlots).ChildRules(timeSlot =>
            {
                timeSlot.RuleFor(x => x.StartTime)
                    .LessThan(x => x.EndTime)
                    .WithMessage("Start time must be before end time.");
            });

            RuleFor(x => x.ScheduleStartDate)
                .NotEmpty()
                .WithMessage("Schedule start date is required.");

            When(x => x.RecurrenceType == RecurrenceType.Weekly, () =>
            {
                RuleFor(x => x.DaysOfWeek)
                    .NotEmpty()
                    .WithMessage("At least one day of week must be selected for weekly recurrence.");

                RuleFor(x => x.ScheduleEndDate)
                    .NotEmpty()
                    .WithMessage("Schedule end date is required for weekly recurrence.")
                    .GreaterThanOrEqualTo(x => x.ScheduleStartDate)
                    .WithMessage("Schedule end date must be greater than or equal to start date.");
            });
        }
    }
}
