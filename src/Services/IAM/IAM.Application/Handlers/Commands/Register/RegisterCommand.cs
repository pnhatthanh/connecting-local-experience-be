using BuildingBlocks.Application.CQRS.Command;
using IAM.Application.DTOs;

namespace IAM.Application.Handlers.Commands.Register
{
    public class RegisterCommand : ICommand<RegisterResponse>
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PasswordConfirm { get; set; } = string.Empty;
    }
}
