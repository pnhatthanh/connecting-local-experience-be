using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Application.Dtos;
using Experience.Application.Dtos;

namespace Experience.Application.Handlers.Queries.GetAllReviews
{
    public record GetAllReviewsQuery(
        bool? isHidden,
        int PageNumber = 1,
        int PageSize = 10
    ) : IQuery<PaginationResult<ReviewDto>>;
}