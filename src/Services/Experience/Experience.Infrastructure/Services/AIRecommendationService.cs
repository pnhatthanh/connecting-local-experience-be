using System.Net.Http.Json;
using Experience.Application.Dtos;
using Experience.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Experience.Infrastructure.Services
{
    public class AIRecommendationService : IAIRecommendationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AIRecommendationService> _logger;
        private readonly string _baseUrl;

        public AIRecommendationService(
            IHttpClientFactory httpClientFactory, 
            IConfiguration configuration, 
            ILogger<AIRecommendationService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _baseUrl = configuration["AIService:BaseUrl"] ?? "http://localhost:8000";
        }

        public async Task<AIRecommendationResponse?> GetRecommendationsAsync(
            Guid userId, 
            int topK = 10, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var url = $"{_baseUrl}/api/recommendations/{userId}?top_k={topK}";
                
                _logger.LogInformation("Calling AI Service for recommendations: {Url}", url);
                
                var response = await httpClient.GetAsync(url, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError(
                        "Failed to get recommendations from AI Service: {StatusCode}, Body: {Body}", 
                        response.StatusCode, 
                        errorContent);
                    return null;
                }

                var result = await response.Content.ReadFromJsonAsync<AIRecommendationResponse>(cancellationToken: cancellationToken);
                
                _logger.LogInformation(
                    "Successfully received {Count} recommendations from AI Service for user {UserId}", 
                    result?.Total ?? 0, 
                    userId);
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling AI Service for user {UserId}", userId);
                return null;
            }
        }

        public async Task<AIRecommendationResponse?> GetPopularExperiencesAsync(
            int topK = 10, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var url = $"{_baseUrl}/api/recommendations/popular?top_k={topK}";
                
                _logger.LogInformation("Calling AI Service for popular experiences: {Url}", url);
                
                var response = await httpClient.GetAsync(url, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError(
                        "Failed to get popular experiences from AI Service: {StatusCode}, Body: {Body}", 
                        response.StatusCode, 
                        errorContent);
                    return null;
                }

                var result = await response.Content.ReadFromJsonAsync<AIRecommendationResponse>(cancellationToken: cancellationToken);
                
                _logger.LogInformation(
                    "Successfully received {Count} popular experiences from AI Service", 
                    result?.Total ?? 0);
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling AI Service for popular experiences");
                return null;
            }
        }
    }
}
