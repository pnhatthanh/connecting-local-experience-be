using Experience.Application.Dtos;
using MediatR;

namespace Experience.Application.Handlers.Commands.UpdateExperience
{
    public class UpdateExperienceCommand : IRequest<ExperienceDto>
    {
        public Guid ExperienceId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? Duration { get; set; }
        public int? MaxParticipants { get; set; }
        public string? CancellationPolicy { get; set; }
        public string? MeetingPoint { get; set; }
    }
}
