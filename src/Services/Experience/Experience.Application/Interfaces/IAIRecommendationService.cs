using Experience.Application.Dtos;

namespace Experience.Application.Interfaces
{
    public interface IAIRecommendationService
    {
        Task<AIRecommendationResponse?> GetRecommendationsAsync(Guid userId, int topK = 10, CancellationToken cancellationToken = default);
        Task<AIRecommendationResponse?> GetPopularExperiencesAsync(int topK = 10, CancellationToken cancellationToken = default);
    }
}
