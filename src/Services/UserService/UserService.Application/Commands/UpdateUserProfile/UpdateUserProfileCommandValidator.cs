using FluentValidation;

namespace UserService.Application.Commands.UpdateUserProfile
{
    public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
    {
        public UpdateUserProfileCommandValidator()
        {
            RuleFor(x => x.FullName)
                .MaximumLength(255)
                .When(x => !string.IsNullOrEmpty(x.FullName))
                .WithMessage("Full name must not exceed 255 characters");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20)
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Phone number must not exceed 20 characters");

            RuleFor(x => x.Nationality)
                .MaximumLength(100)
                .When(x => !string.IsNullOrEmpty(x.Nationality))
                .WithMessage("Nationality must not exceed 100 characters");

            RuleFor(x => x.AvatarUrl)
                .MaximumLength(255)
                .When(x => !string.IsNullOrEmpty(x.AvatarUrl))
                .WithMessage("Avatar URL must not exceed 255 characters");
        }
    }
}
