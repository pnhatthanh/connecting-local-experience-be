using Experience.Domain.Enums;

namespace Experience.Application.Dtos
{
    public class ExperienceDto
    {
        public Guid Id { get; set; }
        public Guid HostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public LocationDto Location { get; set; } = null!;
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public int MaxParticipants { get; set; }
        public string Category { get; set; } = string.Empty;
        public List<string> Amenities { get; set; } = new();
        public string ActivityLevel { get; set; } = string.Empty;
        public string SkillLevel { get; set; } = string.Empty;
        public int MinAge { get; set; }
        public string? Accessibility { get; set; }
        public string Status { get; set; } = string.Empty;
        public string CancellationPolicy { get; set; } = string.Empty;
        public string MeetingPoint { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<ExperienceMediaDto>? Media { get; set; }
        public List<ExperienceItineraryDto>? Itineraries { get; set; }
        public double? Distance { get; set; } 
    }

    public class LocationDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class ExperienceMediaDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public int Order { get; set; }
    }

    public class ExperienceItineraryDto
    {
        public Guid Id { get; set; }
        public int StepNumber { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public LocationDto? Location { get; set; }
    }

    public class ExperienceScheduleDto
    {
        public Guid Id { get; set; }
        public Guid ExperienceId { get; set; }
        public bool IsRecurring { get; set; }
        public string? RecurringPattern { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Timezone { get; set; } = string.Empty;
        public List<ExperienceScheduleSlotDto>? Slots { get; set; }
    }

    public class ExperienceScheduleSlotDto
    {
        public Guid Id { get; set; }
        public Guid ScheduleId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int TotalSlots { get; set; }
        public int AvailableSlots { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
