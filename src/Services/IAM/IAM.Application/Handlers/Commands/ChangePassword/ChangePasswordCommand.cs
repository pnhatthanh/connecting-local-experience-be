using BuildingBlocks.Application.CQRS.Command;

namespace IAM.Application.Handlers.Commands.ChangePassword
{
    public record ChangePasswordCommand(
        string CurrentPassword,
        string NewPassword,
        string ConfirmPassword
    ) : ICommand<bool>;
}
