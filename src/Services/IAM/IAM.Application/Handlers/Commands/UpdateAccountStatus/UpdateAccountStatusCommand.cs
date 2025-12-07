using BuildingBlocks.Application.CQRS.Command;

namespace IAM.Application.Handlers.Commands.UpdateAccountStatus
{
    public record UpdateAccountStatusCommand(
        Guid AccountId, 
        bool IsActive) : ICommand<bool>;
}
