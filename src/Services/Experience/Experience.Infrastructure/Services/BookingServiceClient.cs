using System.Net.Http.Json;
using Experience.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Experience.Infrastructure.Services
{
    public class BookingServiceClient : IBookingServiceClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<BookingServiceClient> _logger;
        private readonly string _baseUrl;

        public BookingServiceClient(
            IHttpClientFactory httpClientFactory, 
            IConfiguration configuration, 
            ILogger<BookingServiceClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _baseUrl = configuration["BookingService:BaseUrl"] ?? "http://localhost:5002";
        }

        public async Task<bool> HasUserBookedExperienceAsync(Guid userId, Guid experienceId)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var url = $"{_baseUrl}/api/bookings/check-completed?userId={userId}&experienceId={experienceId}";
                
                _logger.LogInformation("Calling Booking Service to check if user {UserId} has booked experience {ExperienceId}", 
                    userId, experienceId);
                
                var response = await httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to check booking status: {StatusCode}, Body: {Body}", 
                        response.StatusCode, errorContent);
                    return false;
                }

                var result = await response.Content.ReadFromJsonAsync<bool>();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Booking Service for user {UserId} and experience {ExperienceId}", 
                    userId, experienceId);
                return false;
            }
        }
    }
}
