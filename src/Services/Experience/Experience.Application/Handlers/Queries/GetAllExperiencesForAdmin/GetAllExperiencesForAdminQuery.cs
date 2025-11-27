using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Application.Dtos;
using Experience.Application.Dtos;

namespace Experience.Application.Handlers.Queries.GetAllExperiencesForAdmin
{
    public record GetAllExperiencesForAdminQuery(
        string? SearchTerm,
        string? Status,
        Guid? CategoryId,
        string? SortBy,
        bool IsAscending = false,
        int PageNumber =1,
        int PageSize = 10
    ) : IQuery<PaginationResult<ExperienceSummaryDto>>;
}
