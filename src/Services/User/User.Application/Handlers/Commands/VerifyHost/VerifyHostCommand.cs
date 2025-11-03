using BuildingBlocks.Application.CQRS.Command;
using User.Application.DTOs;

namespace User.Application.Handlers.Commands.VerifyHost
{
    public class VerifyHostCommand : ICommand<HostProfileDto>
    {
        public Guid AccountId { get; set; }
        public string Status { get; set; } = null!;
        public string? Reason { get; set; }
    }
}
