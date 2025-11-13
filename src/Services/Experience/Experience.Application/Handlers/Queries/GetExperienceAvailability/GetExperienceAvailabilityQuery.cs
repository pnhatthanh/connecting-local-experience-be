using BuildingBlocks.Application.CQRS.Query;
using Experience.Application.Dtos;

namespace Experience.Application.Handlers.Queries.GetExperienceAvailability
{
    public class GetExperienceAvailabilityQuery : IQuery<List<ExperienceScheduleSlotDto>>
    {
        public Guid ExperienceId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
