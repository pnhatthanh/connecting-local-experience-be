using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Application.Dtos;
using User.Application.DTOs;

namespace User.Application.Handlers.Queries.GetUserFavorites
{
    public class GetUserFavoritesQuery : IQuery<PaginationResult<UserFavoriteExperienceDto>>
    {
        public Guid UserId { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
