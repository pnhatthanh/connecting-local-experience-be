using BuildingBlocks.Application.CQRS.Command;

namespace IAM.Application.Handlers.Commands.Logout
{
    public record LogoutCommand(string RefreshToken) : ICommand<bool>;
}
