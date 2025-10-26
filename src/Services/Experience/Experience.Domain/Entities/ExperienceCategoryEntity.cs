using BuildingBlocks.Domain.Models;

namespace Experience.Domain.Entities
{
    public class ExperienceCategoryEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<ExperienceEntity> Experiences { get; set; } = [];
    }
}
