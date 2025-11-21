namespace Experience.Application.Dtos
{
    public class ExperienceAvailabilityDto
    {
        public DateOnly Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int SpotsAvailable { get; set; }
    }

    public class ExperienceCalendarDto
    {
        public List<DateAvailabilityDto> Calendar { get; set; } = new();
    }

    public class DateAvailabilityDto
    {
        public DateOnly Date { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public int TotalSpotsAvailable { get; set; }
        public List<TimeSlotAvailabilityDto> TimeSlots { get; set; } = new();
    }

    public class TimeSlotAvailabilityDto
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int SpotsAvailable { get; set; }
        public Guid? SlotId { get; set; } // null nếu slot chưa được tạo trong DB
    }
}
