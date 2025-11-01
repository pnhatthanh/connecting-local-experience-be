using BuildingBlocks.Application.CQRS.Command;

namespace IAM.Application.Handlers.Commands.ResetPassword
{
    public class ResetPasswordCommand : ICommand<bool>
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
