namespace Experience.Application.Dtos
{
    public class ExperienceScheduleDto
    {
        public Guid Id { get; set; }
        public Guid ExperienceId { get; set; }
        public string RecurrenceType { get; set; } = string.Empty;
        public List<DayOfWeek> DaysOfWeek { get; set; } = new();
        public List<TimeSlotDto> TimeSlots { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
