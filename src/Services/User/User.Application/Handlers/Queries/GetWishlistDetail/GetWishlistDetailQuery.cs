using BuildingBlocks.Application.CQRS.Query;
using User.Application.DTOs;

namespace User.Application.Handlers.Queries.GetWishlistDetail
{
    public class GetWishlistDetailQuery : IQuery<WishlistDetailDto>
    {
        public Guid WishlistId { get; set; }
    }
}
