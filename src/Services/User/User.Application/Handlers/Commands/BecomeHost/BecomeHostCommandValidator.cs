using FluentValidation;
using User.Domain.Constants;
using User.Domain.Enums;

namespace User.Application.Handlers.Commands.BecomeHost
{
    public class BecomeHostCommandValidator : AbstractValidator<BecomeHostCommand>
    {
        public BecomeHostCommandValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Phone number is not in a valid format");

            RuleFor(x => x.Avatar)
                .NotNull().WithMessage("Avatar is required")
                .Must(file =>
                {
                    if (file == null) return false;
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    return allowedExtensions.Contains(extension);
                })
                .WithMessage("Avatar must be a valid image file (.jpg, .jpeg, .png)")
                .Must(file =>
                {
                    if (file == null) return false;
                    return file.Length <= 5 * 1024 * 1024; // 5MB
                })
                .WithMessage("Avatar file size must not exceed 5MB");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required")
                .Must(dob =>
                {
                    var age = DateTime.Now.Year - dob.Year;
                    if (DateTime.Now < dob.ToDateTime(TimeOnly.MinValue).AddYears(age))
                        age--;
                    return age >= 18;
                })
                .WithMessage("You must be at least 18 years old to become a host");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Gender is required")
                .Must(gender => new[] { "Male", "Female", "Other" }.Contains(gender))
                .WithMessage("Gender must be Male, Female, or Other");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required")
                .MaximumLength(100).WithMessage("Country cannot exceed 100 characters");

            RuleFor(x => x.Bio)
                .NotEmpty().WithMessage("Bio is required")
                .MinimumLength(50).WithMessage("Bio must be at least 50 characters")
                .MaximumLength(1000).WithMessage("Bio cannot exceed 1000 characters");

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
                .NotNull().WithMessage("Spoken languages is required")
                .Must(languages => languages.Length > 0)
                .WithMessage("At least one spoken language is required");
            
            RuleFor(x => x.TopicsOfInterest)
                .NotNull().WithMessage("Topics of interest is required")
                .Must(topics => topics.Length > 0)
                .WithMessage("At least one topic of interest is required")
                .Must(topics => topics.All(topic => TopicConstants.IsValidTopic(topic)))
                .WithMessage($"One or more topics are invalid. Valid topics are: {string.Join(", ", TopicConstants.ValidTopics)}");

            RuleFor(x => x.Work)
                .NotEmpty().WithMessage("Work is required")
                .MaximumLength(200).WithMessage("Work cannot exceed 200 characters");

            RuleFor(x => x.Education)
                .NotEmpty().WithMessage("Education is required")
                .MaximumLength(200).WithMessage("Education cannot exceed 200 characters");

            RuleFor(x => x.FunFact)
                .NotEmpty().WithMessage("Fun fact is required")
                .MaximumLength(500).WithMessage("Fun fact cannot exceed 500 characters");

            RuleFor(x => x.DesiredHostingStyle)
                .MaximumLength(500).WithMessage("Desired hosting style cannot exceed 500 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.DesiredHostingStyle));

            RuleFor(x => x.ResponseTime)
                .Must(x => Enum.TryParse<ResponseTime>(x, true, out _)).WithMessage("Response time is not valid")
                .When(x => !string.IsNullOrWhiteSpace(x.ResponseTime));

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
