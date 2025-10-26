using BuildingBlocks.Domain.Models;
using Experience.Domain.Enums;
using NetTopologySuite.Geometries;

namespace Experience.Domain.Entities
{
    public class ExperienceEntity : BaseEntity
    {
        public Guid HostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Point Location { get; set; } = null!;
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
        public ExperienceStatus Status { get; set; } = ExperienceStatus.Draft;
        public CancellationPolicyType CancellationPolicy { get; set; } = CancellationPolicyType.AlwaysFreeCancellation;
        public Point MeetingPoint { get; set; } = null!;
        public string MeetingLocation { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public virtual ExperienceCategoryEntity Category { get; set; } = null!;
        public virtual ExperienceScheduleEntity Schedule { get; set; } = null!;
        public virtual ICollection<ExperienceMediaEntity> Media { get; set; } = [];
        public virtual ICollection<ExperienceItineraryEntity> Itineraries { get; set; } = [];
    }
}
