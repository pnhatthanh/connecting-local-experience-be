using BuildingBlocks.Application.CQRS.Query;
using Experience.Application.Dtos;

namespace Experience.Application.Handlers.Queries.GetExperienceAvailability
{
    public record GetExperienceAvailabilityQuery(
        Guid ExperienceId,
        DateOnly StartDate,
        DateOnly EndDate
    ) : IQuery<List<ExperienceAvailabilityDto>>;
}
