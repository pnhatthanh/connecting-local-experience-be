using BuildingBlocks.Domain.Models;
using NetTopologySuite.Geometries;

namespace Experience.Domain.Entities
{
    public class ExperienceItineraryEntity : BaseEntity
    {
        public Guid ExperienceId { get; set; }
        public int StepNumber { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Point? Location { get; set; }
        public virtual ExperienceEntity Experience { get; set; } = null!;
    }
}
