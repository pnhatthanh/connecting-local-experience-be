using BuildingBlocks.Application.CQRS.Command;
using Experience.Application.Dtos;
using Microsoft.AspNetCore.Http;

namespace Experience.Application.Handlers.Commands.CreateExperience
{
    public record CreateExperienceCommand(
        string Title,
        string Description,
        LocationDto Location,
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
        DateTime? EndDate,
        List<IFormFile>? MediaFiles,
        List<CreateExperienceItineraryDto>? Itineraries
    ) : ICommand<ExperienceDto>;
    public record CreateExperienceItineraryDto(
        int StepNumber,
        IFormFile? PhotoFile,
        string Title,
        string Description
    );
}