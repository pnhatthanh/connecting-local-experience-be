using BuildingBlocks.Application.CQRS.Command;
using User.Application.DTOs;

namespace User.Application.Handlers.Commands.CreateWishlist
{
    public class CreateWishlistCommand : ICommand<Guid>
    {
        public string Name { get; set; } = null!;
    }
}
