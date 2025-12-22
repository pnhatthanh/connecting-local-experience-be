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
                var experience = JsonSerializer.Deserialize<ExperienceDto>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                _logger.LogInformation("Successfully retrieved experience: {Title}", experience?.Title);
                return experience;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting experience {ExperienceId}", experienceId);
                return null;
            }
        }

        public async Task<bool> ValidateAvailabilityAsync(Guid experienceId, DateOnly date, TimeSpan startTime, TimeSpan endTime, int adults, int children)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"/api/experiences/{experienceId}/validate-booking?date={date}&startTime={startTime}&endTime={endTime}&adults={adults}&children={children}");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to validate availability for experience {ExperienceId}. Status: {StatusCode}", 
                        experienceId, response.StatusCode);
                    return false;
                }

                var content = await response.Content.ReadAsStringAsync();
                var isAvailable = JsonSerializer.Deserialize<bool>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                return isAvailable;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating availability for experience {ExperienceId}", experienceId);
                return false;
            }
        }
    }
}
