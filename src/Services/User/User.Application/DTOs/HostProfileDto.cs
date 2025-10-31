using User.Domain.Enums;

namespace User.Application.DTOs
{
    public class HostProfileDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
        public string[]? SpokenLanguages { get; set; }
        public string? Location { get; set; }
        public DateTime? HostingSince { get; set; }
        public bool IsVerified { get; set; }
        public string? ResponseTime { get; set; }
        public int TotalBookings { get; set; }
        public int TotalReviews { get; set; }
        public decimal? RatingAvg { get; set; }
        public string? Work { get; set; }
        public string? Education { get; set; }
        public string? FunFact { get; set; }
        public string[]? TopicsOfInterest { get; set; }
        public string? FacebookUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? LinkedInUrl { get; set; }
    }
}
