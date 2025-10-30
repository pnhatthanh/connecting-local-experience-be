using User.Domain.Enums;

namespace User.Application.DTOs
{
    public class HostProfileDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Bio { get; set; }
        public string[]? SpokenLanguages { get; set; }
        public string? Location { get; set; }
        public bool IsVerified { get; set; }
        public VerifyStatus VerifyStatus { get; set; }
        public string? DocumentUrl { get; set; }
        public string? VerifyReason { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public Guid? VerifiedBy { get; set; }
        public decimal ResponseRate { get; set; }
        public string? ResponseTime { get; set; }
        public int TotalExperiences { get; set; }
        public decimal? RatingAvg { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
