using FluentValidation;
using User.Domain.Enums;

namespace User.Application.Handlers.Commands.UpdateUserProfile
{
    public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
    {
        public UpdateUserProfileCommandValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Phone number format is invalid")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .MinimumLength(2).WithMessage("Full name must be at least 2 characters")
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters");
            RuleFor(x => x.Gender)
                .Must(value => Enum.TryParse<Gender>(value, out _))
                .WithMessage("Gender must be a valid value")
                .When(x => !string.IsNullOrWhiteSpace(x.Gender));

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("Date of birth must be in the past")
                .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-120))).WithMessage("Date of birth is not valid")
                .When(x => x.DateOfBirth.HasValue);

            RuleFor(x => x.Country)
                .MaximumLength(100).WithMessage("Country cannot exceed 100 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Country));

            RuleFor(x => x.Avatar)
                .Must(file =>
                {
                    if (file == null) return true;
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    return allowedExtensions.Contains(extension);
                })
                .WithMessage("Avatar must be a valid image file (.jpg, .jpeg, .png, .gif, .webp)")
                .Must(file =>
                {
                    if (file == null) return true;
                    return file.Length <= 10 * 1024 * 1024;
                })
                .WithMessage("Avatar file size must not exceed 10MB")
                .When(x => x.Avatar != null);
        }
    }
}
