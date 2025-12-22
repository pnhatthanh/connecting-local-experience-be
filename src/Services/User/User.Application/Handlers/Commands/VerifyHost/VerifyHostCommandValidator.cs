using FluentValidation;
using User.Domain.Enums;

namespace User.Application.Handlers.Commands.VerifyHost
{
    public class VerifyHostCommandValidator : AbstractValidator<VerifyHostCommand>
    {
        public VerifyHostCommandValidator()
        {
            RuleFor(x => x.AccountId)
                .NotEmpty().WithMessage("Account ID is required");

            RuleFor(x => x.Status)
                .Must(value => Enum.TryParse<VerifyStatus>(value, true, out _))
                .WithMessage("Status must be a valid verification status")
                .NotEqual(VerifyStatus.Pending.ToString()).WithMessage("Cannot set status to Pending");

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Reason is required when rejecting")
                .MinimumLength(10).WithMessage("Reason must be at least 10 characters")
                .When(x => x.Status == VerifyStatus.Rejected.ToString());
        }
    }
}
