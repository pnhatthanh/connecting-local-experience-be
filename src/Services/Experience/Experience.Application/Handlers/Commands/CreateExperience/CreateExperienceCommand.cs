using Experience.Application.Dtos;
using Experience.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Experience.Application.Handlers.Commands.CreateExperience
{
    public class CreateExperienceCommand : IRequest<ExperienceDto>
    {
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
        public Guid CategoryId { get; set; }
        public string ActivityLevel { get; set; } = string.Empty;
        public string SkillLevel { get; set; } = string.Empty;
        public int MinAge { get; set; }
        public string CancellationPolicy { get; set; } = string.Empty;
        public LocationDto MeetingPoint { get; set; } = null!;
        public string MeetingLocation { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public RecurrenceType RecurrenceType { get; set; } = RecurrenceType.Once;
        public List<DayOfWeek> DaysOfWeek { get; set; } = new();
        public List<TimeSlotDto> TimeSlots { get; set; } = new();
        public DateTime ScheduleStartDate { get; set; }
        public DateTime? ScheduleEndDate { get; set; }
        public List<IFormFile>? MediaFiles { get; set; }
        public List<CreateExperienceItineraryDto>? Itineraries { get; set; }
    }

    public class CreateExperienceItineraryDto
    {
        public int StepNumber { get; set; }
        public IFormFile? PhotoFile { get; set; } 
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
