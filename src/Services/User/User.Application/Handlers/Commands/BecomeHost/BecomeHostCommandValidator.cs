using FluentValidation;

namespace User.Application.Handlers.Commands.BecomeHost
{
    public class BecomeHostCommandValidator : AbstractValidator<BecomeHostCommand>
    {
        public BecomeHostCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(x => x.Bio)
                .MinimumLength(50).WithMessage("Bio must be at least 50 characters")
                .MaximumLength(1000).WithMessage("Bio cannot exceed 1000 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Bio));

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required")
                .MaximumLength(200).WithMessage("Location cannot exceed 200 characters");

            RuleFor(x => x.Document)
                .NotNull().WithMessage("Verification document is required for becoming a host")
                .Must(file =>
                {
                    if (file == null) return false;
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
                    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    return allowedExtensions.Contains(extension);
                })
                .WithMessage("Document must be a valid file (.jpg, .jpeg, .png, .pdf)")
                .Must(file =>
                {
                    if (file == null) return false;
                    return file.Length <= 10 * 1024 * 1024; // 10MB
                })
                .WithMessage("Document file size must not exceed 10MB");

            RuleFor(x => x.SpokenLanguages)
                .Must(languages => languages == null || languages.Length > 0)
                .WithMessage("At least one spoken language is required")
                .When(x => x.SpokenLanguages != null);
        }
    }
}
