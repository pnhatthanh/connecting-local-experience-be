using BuildingBlocks.Application.CQRS.Query;
using Experience.Application.Dtos;

namespace Experience.Application.Handlers.Queries.GetPopularExperience
{
    public record GetPopularExperienceQuery(
        int TopK = 10
    ) : IQuery<List<ExperienceRecommendationDto>>;
}
