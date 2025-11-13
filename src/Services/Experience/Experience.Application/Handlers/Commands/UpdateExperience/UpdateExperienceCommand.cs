using BuildingBlocks.Application.CQRS.Command;
using Experience.Application.Dtos;
using Microsoft.AspNetCore.Http;
using NetTopologySuite.Geometries;

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
        string Language,
        string RecurrenceType,
        List<DayOfWeek> DaysOfWeek,
        List<TimeSlotDto> TimeSlots,
        DateOnly StartDate,
        DateOnly? EndDate,
        List<Guid>? KeepMediaIds,
        List<IFormFile>? NewMediaFiles,
        List<UpdateExperienceItineraryDto>? Itineraries
    ) : ICommand<ExperienceDto>;
    
    public record UpdateExperienceItineraryDto(
        Guid? Id,  
        int StepNumber,
        IFormFile? PhotoFile,  
        string Title,
        string Description
    );
}
