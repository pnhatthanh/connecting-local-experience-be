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

            RuleFor(x => x.Work)
                .MaximumLength(200).WithMessage("Work cannot exceed 200 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Work));

            RuleFor(x => x.Education)
                .MaximumLength(200).WithMessage("Education cannot exceed 200 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Education));

            RuleFor(x => x.FunFact)
                .MaximumLength(500).WithMessage("Fun fact cannot exceed 500 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.FunFact));

            RuleFor(x => x.FacebookUrl)
                .Must(BeAValidUrl).WithMessage("Facebook URL is not valid")
                .When(x => !string.IsNullOrWhiteSpace(x.FacebookUrl));

            RuleFor(x => x.InstagramUrl)
                .Must(BeAValidUrl).WithMessage("Instagram URL is not valid")
                .When(x => !string.IsNullOrWhiteSpace(x.InstagramUrl));

            RuleFor(x => x.LinkedInUrl)
                .Must(BeAValidUrl).WithMessage("LinkedIn URL is not valid")
                .When(x => !string.IsNullOrWhiteSpace(x.LinkedInUrl));
        }

        private bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return true;
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
