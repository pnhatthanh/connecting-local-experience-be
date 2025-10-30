using BuildingBlocks.Application.CQRS.Command;

namespace User.Application.Handlers.Commands.RemoveFavorite
{
    public class RemoveFavoriteCommand : ICommand<bool>
    {
        public Guid UserId { get; set; }
        public Guid ExperienceId { get; set; }
    }
}
