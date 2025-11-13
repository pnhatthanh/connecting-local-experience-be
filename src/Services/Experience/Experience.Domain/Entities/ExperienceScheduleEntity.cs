using BuildingBlocks.Domain.Models;
using Experience.Domain.Enums;

namespace Experience.Domain.Entities
{
    public class ExperienceScheduleEntity : BaseEntity
    {
        public Guid ExperienceId { get; set; }
        public RecurrenceType RecurrenceType { get; set; } = RecurrenceType.Once;
        public List<DayOfWeek> DaysOfWeek { get; set; } = new();
        public List<ScheduleTimeSlot> TimeSlots { get; set; } = new();
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public virtual ExperienceEntity Experience { get; set; } = null!;
        public virtual ICollection<ExperienceScheduleSlotEntity> Slots { get; set; } = new List<ExperienceScheduleSlotEntity>();
    }
    public class ScheduleTimeSlot
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
