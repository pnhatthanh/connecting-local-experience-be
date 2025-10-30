using FluentValidation;

namespace User.Application.Handlers.Commands.AddFavorite
{
    public class AddFavoriteCommandValidator : AbstractValidator<AddFavoriteCommand>
    {
        public AddFavoriteCommandValidator()
        {
            RuleFor(x => x.ExperienceId)
                .NotEmpty().WithMessage("Experience ID is required");
        }
    }
}
