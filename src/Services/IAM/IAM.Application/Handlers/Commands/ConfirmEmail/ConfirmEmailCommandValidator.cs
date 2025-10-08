using FluentValidation;

namespace IAM.Application.Handlers.Commands.ConfirmEmailCommand
{
    public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email is not valid");

            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required");
        }
    }
}
