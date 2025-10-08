using BuildingBlocks.Application.CQRS.Command;

namespace IAM.Application.Handlers.Commands.ConfirmEmailCommand
{
    public class ConfirmEmailCommand : ICommand<bool>
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
