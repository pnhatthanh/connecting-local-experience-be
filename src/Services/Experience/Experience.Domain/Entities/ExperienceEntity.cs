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
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public int MaxParticipants { get; set; }
        public ExperienceCategory Category { get; set; }
        public ActivityLevel ActivityLevel { get; set; }
        public SkillLevel SkillLevel { get; set; }
        public int MinAge { get; set; }
        public string? Accessibility { get; set; }
        public List<string> Amenities { get; set; } = new();
        public ExperienceStatus Status { get; set; } = ExperienceStatus.Draft;
        public string CancellationPolicy { get; set; } = string.Empty;
        public string MeetingPoint { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public virtual ICollection<ExperienceScheduleEntity> Schedules { get; set; } = [];
        public virtual ICollection<ExperienceMediaEntity> Media { get; set; } = [];
        public virtual ICollection<ExperienceItineraryEntity> Itineraries { get; set; } = [];
    }
}
