using Experience.Application.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Experience.Application.Handlers.Commands.CreateExperience
{
    public class CreateExperienceCommand : IRequest<ExperienceDto>
    {
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
        public string CancellationPolicy { get; set; } = string.Empty;
        public string MeetingPoint { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        
        public List<IFormFile>? MediaFiles { get; set; }
        public List<CreateExperienceItineraryDto>? Itineraries { get; set; }
    }

    public class CreateExperienceItineraryDto
    {
        public int StepNumber { get; set; }
        public IFormFile? PhotoFile { get; set; } 
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public LocationDto? Location { get; set; }
    }
}
