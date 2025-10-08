using BuildingBlocks.Application.CQRS.Command;

namespace IAM.Application.Handlers.Commands.ResetPasswordCommand
{
    public class ResetPasswordCommand : ICommand<bool>
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
