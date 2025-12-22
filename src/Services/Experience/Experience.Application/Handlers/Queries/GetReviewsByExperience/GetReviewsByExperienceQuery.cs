using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Application.Dtos;
using Experience.Application.Dtos;

namespace Experience.Application.Handlers.Queries.GetReviewsByExperience
{
    public record GetReviewsByExperienceQuery(
        Guid ExperienceId,
        int PageNumber = 1,
        int PageSize = 10
    ) : IQuery<PaginationResult<ReviewDto>>;
}
