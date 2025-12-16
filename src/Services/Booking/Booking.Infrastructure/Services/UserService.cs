using Booking.Application.Dtos;
using Booking.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Net.Http.Json;

namespace Booking.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserService> _logger;

        public UserService(HttpClient httpClient, ILogger<UserService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<UserDto?> GetUserAsync(Guid userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/users/{userId}");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to get user {UserId}. Status: {StatusCode}", 
                        userId, response.StatusCode);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                
                // Parse the response which has { "success": true, "data": {...} } format
                var jsonDoc = JsonSerializer.Deserialize<JsonElement>(content);
                if (jsonDoc.TryGetProperty("data", out var dataElement))
                {
                    var user = JsonSerializer.Deserialize<UserDto>(dataElement.GetRawText(), 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    _logger.LogInformation("Successfully retrieved user: {FullName}", user?.FullName);
                    return user;
                }

                _logger.LogWarning("User API response doesn't contain 'data' property");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {UserId}", userId);
                return null;
            }
        }

        public async Task<List<HostProfileDto>> GetHostProfilesAsync(List<Guid> hostIds, CancellationToken cancellationToken = default)
        {
            try
            {
                var idsQuery = string.Join("&", hostIds.Select(id => $"ids={id}"));
                var response = await _httpClient.GetAsync($"/api/hosts/batch?{idsQuery}", cancellationToken);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to get host profiles. Status: {StatusCode}", response.StatusCode);
                    return new List<HostProfileDto>();
                }

                var hostProfiles = await response.Content.ReadFromJsonAsync<List<HostProfileDto>>(cancellationToken);
                _logger.LogInformation("Successfully retrieved {Count} host profiles", hostProfiles?.Count ?? 0);
                return hostProfiles ?? new List<HostProfileDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting host profiles");
                return new List<HostProfileDto>();
            }
        }
    }
}
