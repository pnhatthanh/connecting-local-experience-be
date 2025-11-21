using BuildingBlocks.Application.CQRS.Command;

namespace User.Application.Handlers.Commands.RemoveExperienceFromWishlist
{
    public class RemoveExperienceFromWishlistCommand : ICommand<bool>
    {
        public Guid WishlistId { get; set; }
        public Guid ExperienceId { get; set; }
    }
}
