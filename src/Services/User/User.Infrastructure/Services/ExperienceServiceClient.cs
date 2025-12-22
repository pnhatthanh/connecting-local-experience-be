using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using User.Application.DTOs;
using User.Application.Interfaces;

namespace User.Infrastructure.Services
{
    public class ExperienceServiceClient : IExperienceServiceClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ExperienceServiceClient> _logger;
        private readonly string _baseUrl;

        public ExperienceServiceClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<ExperienceServiceClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _baseUrl = configuration["ExperienceService:BaseUrl"] ?? "http://localhost:5003";
        }

        public async Task<List<ExperienceSummaryDto>> GetExperiencesByIdsAsync(List<Guid> experienceIds)
        {
            try
            {
                if (!experienceIds.Any())
                    return new List<ExperienceSummaryDto>();

                var httpClient = _httpClientFactory.CreateClient();
                
                var uriBuilder = new UriBuilder($"{_baseUrl}/api/experiences/batch");
                var queryParams = string.Join("&", experienceIds.Select(id => $"ids={id}"));
                uriBuilder.Query = queryParams;
                
                _logger.LogInformation("Calling Experience Service: {Url}", uriBuilder.Uri.ToString());
                var response = await httpClient.GetAsync(uriBuilder.Uri);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to fetch experiences from Experience Service: {StatusCode}, Body: {Body}", response.StatusCode, errorContent);
                    return new List<ExperienceSummaryDto>();
                }

                var experiences = await response.Content.ReadFromJsonAsync<List<ExperienceSummaryDto>>();
                return experiences ?? new List<ExperienceSummaryDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Experience Service");
                return new List<ExperienceSummaryDto>();
            }
        }

        public async Task<bool> CheckExperienceExistsAsync(Guid experienceId)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var url = $"{_baseUrl}/api/experiences/{experienceId}";
                var response = await httpClient.GetAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking experience existence");
                return false;
            }
        }
    }
}
