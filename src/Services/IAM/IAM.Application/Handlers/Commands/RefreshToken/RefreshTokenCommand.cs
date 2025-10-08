using BuildingBlocks.Application.CQRS.Command;
using IAM.Application.DTOs;

namespace IAM.Application.Handlers.Commands.RefreshTokenCommand
{
    public class RefreshTokenCommand : ICommand<TokenResponse>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}