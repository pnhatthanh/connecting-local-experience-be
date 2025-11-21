using BuildingBlocks.Application.CQRS.Query;
using User.Application.DTOs;

namespace User.Application.Handlers.Queries.GetUserWishlists
{
    public class GetUserWishlistsQuery : IQuery<List<WishlistDto>>
    {
    }
}
