using BuildingBlocks.Application.CQRS.Query;
using Experience.Application.Dtos;

namespace Experience.Application.Handlers.Queries.GetExperience
{
    public record GetExperienceQuery(
        Guid ExperienceId
    ) : IQuery<ExperienceDto?>;
}
