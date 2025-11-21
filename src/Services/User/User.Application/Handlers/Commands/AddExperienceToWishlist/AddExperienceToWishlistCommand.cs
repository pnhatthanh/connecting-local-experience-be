using BuildingBlocks.Application.CQRS.Command;

namespace User.Application.Handlers.Commands.AddExperienceToWishlist
{
    public class AddExperienceToWishlistCommand : ICommand<bool>
    {
        public Guid WishlistId { get; set; }
        public Guid ExperienceId { get; set; }
    }
}
