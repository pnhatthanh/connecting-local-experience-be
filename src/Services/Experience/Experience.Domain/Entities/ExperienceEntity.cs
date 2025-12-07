using BuildingBlocks.Domain.Models;
using Experience.Domain.Enums;

namespace Experience.Domain.Entities
{
    public class ExperienceEntity : BaseEntity
    {
        public Guid HostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public decimal AdultPrice { get; set; } 
        public decimal ChildPrice { get; set; }
        public int Duration { get; set; }
        public int MaxParticipants { get; set; }
        public Guid CategoryId { get; set; }
        public ActivityLevel ActivityLevel { get; set; }
        public SkillLevel SkillLevel { get; set; }
        public int MinAge { get; set; }
        public string? Accessibility { get; set; }
        public ExperienceStatus Status { get; set; } = ExperienceStatus.Pending;
        public CancellationPolicyType CancellationPolicy { get;  set; } = CancellationPolicyType.AlwaysFreeCancellation;
        public string Language { get; set; } = string.Empty;
        public int TotalReviews { get; set; } = 0;
        public double AverageRating { get; set; } = 0.0;
        public virtual ExperienceCategoryEntity Category { get; set; } = null!;
        public virtual ExperienceScheduleEntity Schedule { get; set; } = null!;
        public virtual ICollection<ExperienceMediaEntity> Media { get; set; } = [];
        public virtual ICollection<ExperienceItineraryEntity> Itineraries { get; set; } = [];
        public virtual ICollection<ReviewEntity> Reviews { get; set; } = [];
    }
}
