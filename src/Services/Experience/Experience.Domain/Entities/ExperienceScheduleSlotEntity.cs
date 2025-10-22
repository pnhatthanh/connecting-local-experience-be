using BuildingBlocks.Domain.Models;
using Experience.Domain.Enums;

namespace Experience.Domain.Entities
{
    public class ExperienceScheduleSlotEntity : BaseEntity
    {
        public Guid ScheduleId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int TotalSlots { get; set; }
        public int AvailableSlots { get; set; }
        public SlotStatus Status { get; set; } = SlotStatus.Open;
        public string? CancelReason { get; set; }
        public DateTime? CancelledAt { get; set; }
        public virtual ExperienceScheduleEntity Schedule { get; set; } = null!;
    }
}
