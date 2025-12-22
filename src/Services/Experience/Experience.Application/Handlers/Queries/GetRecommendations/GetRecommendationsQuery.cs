using BuildingBlocks.Application.CQRS.Query;
using Experience.Application.Dtos;

namespace Experience.Application.Handlers.Queries.GetRecommendations
{
    public record GetRecommendationsQuery(
        int TopK = 10
    ) : IQuery<List<ExperienceRecommendationDto>>;
}
