using BuildingBlocks.Domain.Models;

namespace Experience.Domain.Entities
{
    public class ExperienceMediaEntity : BaseEntity
    {
        public Guid ExperienceId { get; set; }
        public string Url { get; set; } = string.Empty;
        public int Order { get; set; }
        public virtual ExperienceEntity Experience { get; set; } = null!;
    }
}
