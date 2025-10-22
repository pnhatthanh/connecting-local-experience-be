using BuildingBlocks.Domain.Models;

namespace Experience.Domain.Entities
{
    public class ExperienceScheduleEntity : BaseEntity
    {
        public Guid ExperienceId { get; set; }
        public bool IsRecurring { get; set; } = false;
        public string? RecurringPattern { get; set; } 
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Timezone { get; set; } = "Asia/Ho_Chi_Minh";
        public virtual ExperienceEntity Experience { get; set; } = null!;
        public virtual ICollection<ExperienceScheduleSlotEntity> Slots { get; set; } = new List<ExperienceScheduleSlotEntity>();
    }
}
