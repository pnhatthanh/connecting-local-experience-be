using BuildingBlocks.Application.CQRS.Command;

namespace User.Application.Handlers.Commands.AddFavorite
{
    public class AddFavoriteCommand : ICommand<bool>
    {
        public Guid UserId { get; set; }
        public Guid ExperienceId { get; set; }
    }
}
