using BuildingBlocks.Application.CQRS.Command;

namespace UserService.Application.Commands.Admin.ActivateUser
{
    public class ActivateUserCommand : ICommand<bool>
    {
        public Guid UserId { get; set; }
        public ActivateUserCommand(Guid userId)
        {
            UserId = userId;
        }
    }
}
