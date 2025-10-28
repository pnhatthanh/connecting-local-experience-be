using BuildingBlocks.Application.CQRS.Command;
using Experience.Application.Dtos;

namespace Experience.Application.Handlers.Commands.UpdateExperience
{
    public record UpdateExperienceCommand(
        Guid ExperienceId,
        string Title,
        string Description,
        string Address,
        string District,
        string City,
        string Country,
        decimal AdultPrice,
        decimal ChildPrice,
        int Duration,
        int MaxParticipants,
        Guid CategoryId,
        string ActivityLevel,
        string SkillLevel,
        int MinAge,
        string CancellationPolicy,
        LocationDto MeetingPoint,
        string MeetingLocation,
        string Language,
        string RecurrenceType,
        List<DayOfWeek> DaysOfWeek,
        List<TimeSlotDto> TimeSlots,
        DateTime StartDate,
        DateTime? EndDate
    ) : ICommand<ExperienceDto>;
}
