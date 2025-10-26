namespace Experience.Application.Dtos
{
    public class ExperienceDto
    {
        public Guid Id { get; set; }
        public Guid HostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public LocationDto Location { get; set; } = null!;
        public string Address { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public decimal AdultPrice { get; set; }
        public decimal ChildPrice { get; set; }
        public int Duration { get; set; }
        public int MaxParticipants { get; set; }
        public ExperienceCategoryDto Category { get; set; } = null!;
        public string ActivityLevel { get; set; } = string.Empty;
        public string SkillLevel { get; set; } = string.Empty;
        public int MinAge { get; set; }
        public string? Accessibility { get; set; }
        public string Status { get; set; } = string.Empty;
        public string CancellationPolicy { get; set; } = string.Empty;
        public LocationDto MeetingPoint { get; set; } = null!;
        public string MeetingLocation { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<ExperienceMediaDto>? Media { get; set; }
        public List<ExperienceItineraryDto>? Itineraries { get; set; }
        public double? Distance { get; set; } 
    }
}
