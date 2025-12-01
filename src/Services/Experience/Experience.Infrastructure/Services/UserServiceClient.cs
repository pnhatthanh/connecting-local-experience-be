using System.Net.Http.Json;
using Experience.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Experience.Infrastructure.Services
{
    public class UserServiceClient : IUserServiceClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<UserServiceClient> _logger;
        private readonly string _baseUrl;

        public UserServiceClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<UserServiceClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _baseUrl = configuration["UserService:BaseUrl"] ?? "http://localhost:5003";
        }

        public async Task<List<Guid>> CheckExperiencesInWishlistAsync(Guid userId, List<Guid> experienceIds)
        {
            try
            {
                if (!experienceIds.Any())
                    return new List<Guid>();

                var httpClient = _httpClientFactory.CreateClient();
                
                var uriBuilder = new UriBuilder($"{_baseUrl}/api/wishlists/check-experiences");
                var queryParams = string.Join("&", experienceIds.Select(id => $"experienceIds={id}"));
                uriBuilder.Query = $"userId={userId}&{queryParams}";
                
                _logger.LogInformation("Calling User Service: {Url}", uriBuilder.Uri.ToString());
                var response = await httpClient.GetAsync(uriBuilder.Uri);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to fetch wishlist from User Service: {StatusCode}, Body: {Body}", response.StatusCode, errorContent);
                    return new List<Guid>();
                }

                var favoriteIds = await response.Content.ReadFromJsonAsync<List<Guid>>();
                return favoriteIds ?? new List<Guid>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling User Service");
                return new List<Guid>();
            }
        }
    }
}
