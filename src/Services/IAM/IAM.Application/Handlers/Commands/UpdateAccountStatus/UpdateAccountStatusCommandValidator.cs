using FluentValidation;

namespace IAM.Application.Handlers.Commands.UpdateAccountStatus
{
    public class UpdateAccountStatusCommandValidator : AbstractValidator<UpdateAccountStatusCommand>
    {
        public UpdateAccountStatusCommandValidator()
        {
            RuleFor(x => x.AccountId)
                .NotEmpty()
                .WithMessage("Account ID is required");
        }
    }
}
