using BuildingBlocks.Application.CQRS.Command;

namespace User.Application.Handlers.Commands.DeleteWishlist
{
    public class DeleteWishlistCommand : ICommand<bool>
    {
        public Guid WishlistId { get; set; }
    }
}
