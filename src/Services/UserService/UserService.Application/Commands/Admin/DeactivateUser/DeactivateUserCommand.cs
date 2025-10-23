using BuildingBlocks.Application.CQRS.Command;

namespace UserService.Application.Commands.Admin.DeactivateUser
{
    public class DeactivateUserCommand : ICommand<bool>
    {
        public Guid UserId { get; set; }
        public DeactivateUserCommand(Guid userId)
        {
            UserId = userId;
        }
    }
}
