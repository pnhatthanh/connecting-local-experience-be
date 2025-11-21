using FluentValidation;

namespace User.Application.Handlers.Commands.AddExperienceToWishlist
{
    public class AddExperienceToWishlistCommandValidator : AbstractValidator<AddExperienceToWishlistCommand>
    {
        public AddExperienceToWishlistCommandValidator()
        {
            RuleFor(x => x.WishlistId)
                .NotEmpty().WithMessage("WishlistId is required");

            RuleFor(x => x.ExperienceId)
                .NotEmpty().WithMessage("ExperienceId is required");
        }
    }
}
