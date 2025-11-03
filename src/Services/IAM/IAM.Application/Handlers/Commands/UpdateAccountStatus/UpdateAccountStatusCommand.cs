using BuildingBlocks.Application.CQRS.Command;

namespace IAM.Application.Handlers.Commands.UpdateAccountStatus
{
    public class UpdateAccountStatusCommand : ICommand<bool>
    {
        public Guid AccountId { get; set; }
        public bool IsActive { get; set; }
    }
}
