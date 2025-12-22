using FluentValidation;

namespace User.Application.Handlers.Commands.CreateWishlist
{
    public class CreateWishlistCommandValidator : AbstractValidator<CreateWishlistCommand>
    {
        public CreateWishlistCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Wishlist name is required")
                .MaximumLength(100).WithMessage("Wishlist name must not exceed 100 characters");
        }
    }
}
