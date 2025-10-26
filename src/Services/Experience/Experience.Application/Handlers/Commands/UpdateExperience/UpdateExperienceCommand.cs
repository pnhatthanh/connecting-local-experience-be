using Experience.Application.Dtos;
using Experience.Domain.Enums;
using MediatR;

namespace Experience.Application.Handlers.Commands.UpdateExperience
{
    public class UpdateExperienceCommand : IRequest<ExperienceDto>
    {
        public Guid ExperienceId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public decimal AdultPrice { get; set; }
        public decimal ChildPrice { get; set; }
        public int Duration { get; set; }
        public int MaxParticipants { get; set; }
        public Guid CategoryId { get; set; }
        public int MinAge { get; set; }
        public CancellationPolicyType CancellationPolicy { get; set; } = CancellationPolicyType.AlwaysFreeCancellation;
        public LocationDto MeetingPoint { get; set; } = null!;
        public string MeetingLocation { get; set; } = string.Empty;
        
        // Schedule
        public RecurrenceType RecurrenceType { get; set; } = RecurrenceType.Once;
        public List<DayOfWeek> DaysOfWeek { get; set; } = new();
        public List<TimeSlotDto> TimeSlots { get; set; } = new();
        public DateTime ScheduleStartDate { get; set; }
        public DateTime? ScheduleEndDate { get; set; }
    }
}
