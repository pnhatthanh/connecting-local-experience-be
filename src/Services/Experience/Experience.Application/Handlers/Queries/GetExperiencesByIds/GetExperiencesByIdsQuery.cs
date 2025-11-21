using BuildingBlocks.Application.CQRS.Query;
using Experience.Application.Dtos;

namespace Experience.Application.Handlers.Queries.GetExperiencesByIds
{
    public record GetExperiencesByIdsQuery(List<Guid> Ids) : IQuery<List<ExperienceSummaryDto>>;
}
