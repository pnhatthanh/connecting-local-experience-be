namespace Experience.Application.Dtos
{
    public class ExperienceAvailabilityDto
    {
        public DateOnly Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int SpotsAvailable { get; set; }
    }
}
