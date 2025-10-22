using FluentValidation;

namespace Experience.Application.Handlers.Commands.CreateExperience
{
    public class CreateExperienceCommandValidator : AbstractValidator<CreateExperienceCommand>
    {
        public CreateExperienceCommandValidator()
        {
            RuleFor(x => x.HostId).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Location).NotNull();
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.Duration).GreaterThan(0);
            RuleFor(x => x.MaxParticipants).GreaterThan(0);
            RuleFor(x => x.Category).NotEmpty();
            RuleFor(x => x.ActivityLevel).NotEmpty();
            RuleFor(x => x.SkillLevel).NotEmpty();
            RuleFor(x => x.MinAge).GreaterThanOrEqualTo(0);
            RuleFor(x => x.CancellationPolicy).NotEmpty().MaximumLength(255);
            RuleFor(x => x.MeetingPoint).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Language).NotEmpty().MaximumLength(50);
        }
    }
}
