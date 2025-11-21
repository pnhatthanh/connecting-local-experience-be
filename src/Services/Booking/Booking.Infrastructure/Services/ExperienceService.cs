using Booking.Application.Dtos;
using Booking.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Booking.Infrastructure.Services
{
    public class ExperienceService : IExperienceService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExperienceService> _logger;

        public ExperienceService(HttpClient httpClient, ILogger<ExperienceService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ExperienceDto?> GetExperienceAsync(Guid experienceId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/experiences/{experienceId}");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to get experience {ExperienceId}. Status: {StatusCode}", 
                        experienceId, response.StatusCode);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<ExperienceDto>>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                return result?.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting experience {ExperienceId}", experienceId);
                return null;
            }
        }

        public async Task<bool> ValidateAvailabilityAsync(Guid experienceId, DateTime startTime, DateTime endTime, int adults, int children)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"/api/experiences/{experienceId}/availability?startTime={startTime:o}&endTime={endTime:o}&adults={adults}&children={children}");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to validate availability for experience {ExperienceId}. Status: {StatusCode}", 
                        experienceId, response.StatusCode);
                    return false;
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<bool>>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                return result?.Data ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating availability for experience {ExperienceId}", experienceId);
                return false;
            }
        }

        private class ApiResponse<T>
        {
            public bool Success { get; set; }
            public T? Data { get; set; }
            public string? Message { get; set; }
        }
    }
}
