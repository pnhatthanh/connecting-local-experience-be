using BuildingBlocks.Application.CQRS.Command;
using IAM.Application.DTOs;

namespace IAM.Application.Handlers.Commands.LoginCommand
{
    public class LoginCommand : ICommand<TokenResponse>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
