using System.Text.Json.Serialization;

namespace Experience.Application.Dtos
{
    /// <summary>
    /// DTO for AI recommendation response
    /// </summary>
    public class RecommendationItemDto
    {
        [JsonPropertyName("experience_id")]
        public string ExperienceId { get; set; } = string.Empty;
        
        [JsonPropertyName("score")]
        public double Score { get; set; }
        
        [JsonPropertyName("reason")]
        public string Reason { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response from AI Service - chỉ chứa IDs + scores
    /// </summary>
    public class AIRecommendationResponse
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = string.Empty;
        
        [JsonPropertyName("recommendations")]
        public List<RecommendationItemDto> Recommendations { get; set; } = new();
        
        [JsonPropertyName("total")]
        public int Total { get; set; }
        
        [JsonPropertyName("generated_at")]
        public string GeneratedAt { get; set; } = string.Empty;
        
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;
    }

    /// <summary>
    /// Final recommendation DTO trả về cho client - đã có full data
    /// </summary>
    public class ExperienceRecommendationDto
    {
        public ExperienceSummaryDto Experience { get; set; } = null!;
        public double Score { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
