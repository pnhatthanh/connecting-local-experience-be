using BuildingBlocks.Domain.Models;

namespace Experience.Domain.Entities
{
    public class ReviewEntity : BaseEntity
    {
        public Guid ExperienceId { get; set; }
        public Guid UserId { get; set; }
        public Guid HostId { get; set; }
        public int Rating { get; set; } // 1-5
        public string Description { get; set; } = string.Empty;
        public bool IsHidden { get; set; } = false;
        
        // Navigation properties
        public virtual ExperienceEntity Experience { get; set; } = null!;
    }
}
