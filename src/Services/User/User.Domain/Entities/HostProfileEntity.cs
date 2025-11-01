using BuildingBlocks.Domain.Models;
using User.Domain.Enums;

namespace User.Domain.Entities
{
    public class HostProfileEntity : BaseEntity
    {
        public Guid UserId { get; set; }
        public string? Bio { get; set; }
        public string[] SpokenLanguages { get; set; } = [];
        public string? Location { get; set; }
        public DateTime? HostingSince { get; set; }
        public bool IsVerified { get; set; } = false;
        public VerifyStatus VerifyStatus { get; set; } = VerifyStatus.Pending;
        public string? DocumentUrl { get; set; } //Passport images
        public string? VerifyReason { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public ResponseTime? ResponseTime { get; set; } // e.g., "within an hour", "within a few hours"
        public int TotalExperiences { get; set; } = 0;
        public int TotalBookings { get; set; } = 0;
        public int TotalReviews { get; set; } = 0;
        public decimal? RatingAvg { get; set; } 
        public string? Work { get; set; }
        public string? Education { get; set; }
        public string? FunFact { get; set; }
        public string[] TopicsOfInterest { get; set; } = [];
        public string? DesiredHostingStyle { get; set; }
        public string? FacebookUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? LinkedInUrl { get; set; }
        public virtual UserEntity User { get; set; } = null!;
    }
}
