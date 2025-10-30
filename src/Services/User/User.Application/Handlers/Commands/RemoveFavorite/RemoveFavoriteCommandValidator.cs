using FluentValidation;

namespace User.Application.Handlers.Commands.RemoveFavorite
{
    public class RemoveFavoriteCommandValidator : AbstractValidator<RemoveFavoriteCommand>
    {
        public RemoveFavoriteCommandValidator()
        {
            RuleFor(x => x.ExperienceId)
                .NotEmpty().WithMessage("Experience ID is required");
        }
    }
}
