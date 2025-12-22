namespace Experience.Application.Dtos
{
    public class ExperienceScheduleSlotDto
    {
        public Guid Id { get; set; }
        public Guid ScheduleId { get; set; }
        public DateOnly Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int TotalSlots { get; set; }
        public int AvailableSlots { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
