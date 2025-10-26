namespace Experience.Application.Dtos
{
    public class ExperienceItineraryDto
    {
        public Guid Id { get; set; }
        public int StepNumber { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
