using Experience.Application.Dtos;
using Experience.Domain.Enums;
using FluentValidation;

namespace Experience.Application.Handlers.Commands.CreateExperience
{
    public class CreateExperienceCommandValidator : AbstractValidator<CreateExperienceCommand>
    {
        public CreateExperienceCommandValidator()
        {       
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MaximumLength(255)
                .WithMessage("Title must not exceed 255 characters.");
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required.");
            RuleFor(x => x.Location)
                .NotNull()
                .WithMessage("Location is required.");
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
            RuleFor(x => x.ActivityLevel)
                .NotEmpty()
                .WithMessage("Activity level is required.");
            RuleFor(x => x.SkillLevel)
                .NotEmpty()
                .WithMessage("Skill level is required.");
            RuleFor(x => x.MinAge)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Minimum age must be greater than or equal to 0.");
            RuleFor(x => x.CancellationPolicy)
                .NotEmpty()
                .WithMessage("Cancellation policy is required.")
                .Must(policy => Enum.TryParse<CancellationPolicyType>(policy, true, out _))
                .WithMessage("Invalid cancellation policy.");
            RuleFor(x => x.MeetingPoint)
                .NotNull()
                .WithMessage("Meeting point is required.");
            RuleFor(x => x.MeetingLocation)
                .NotEmpty()
                .WithMessage("Meeting location is required.")
                .MaximumLength(500)
                .WithMessage("Meeting location must not exceed 500 characters.");
            RuleFor(x => x.Language)
                .NotEmpty()
                .WithMessage("Language is required.")
                .MaximumLength(50)
                .WithMessage("Language must not exceed 50 characters.");
            RuleFor(x => x.RecurrenceType)
                .NotEmpty()
                .WithMessage("Recurrence type is required.")
                .Must(type => Enum.TryParse<RecurrenceType>(type, true, out _))
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
            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage("Start date is required.");
            When(x => x.RecurrenceType == RecurrenceType.Weekly.ToString(), () =>
            {
                RuleFor(x => x.DaysOfWeek)
                    .NotEmpty()
                    .WithMessage("At least one day of week must be selected for weekly recurrence.");

                RuleFor(x => x.EndDate)
                    .NotEmpty()
                    .WithMessage("End date is required for weekly recurrence.")
                    .GreaterThanOrEqualTo(x => x.StartDate)
                    .WithMessage("End date must be greater than or equal to start date.");
            });
            
            RuleFor(x => x.TimeSlots)
                .Must(timeSlots => !HasOverlappingTimeSlots(timeSlots))
                .When(x => x.TimeSlots != null && x.TimeSlots.Any())
                .WithMessage("Time slots cannot overlap with each other.");
        }
        
        private bool HasOverlappingTimeSlots(List<TimeSlotDto> timeSlots)
        {
            if (timeSlots == null || timeSlots.Count <= 1)
                return false;
                
            var sortedSlots = timeSlots.OrderBy(t => t.StartTime).ToList();
            
            for (int i = 0; i < sortedSlots.Count - 1; i++)
            {
                var currentSlot = sortedSlots[i];
                var nextSlot = sortedSlots[i + 1];

                if (currentSlot.EndTime > nextSlot.StartTime)
                    return true;
            }
            
            return false;
        }
    }
}
