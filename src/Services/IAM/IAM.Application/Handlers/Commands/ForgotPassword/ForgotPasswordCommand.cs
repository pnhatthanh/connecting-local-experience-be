using BuildingBlocks.Application.CQRS.Command;

namespace IAM.Application.Handlers.Commands.ForgotPassword
{
    public class ForgotPasswordCommand : ICommand<bool>
    {
        public string Email { get; set; } = string.Empty;
    }
}
