using BuildingBlocks.Application.CQRS.Query;
using Experience.Application.Dtos;

namespace Experience.Application.Handlers.Queries.GetExperienceAvailability
{
    public class GetExperienceAvailabilityQuery : IQuery<List<ExperienceScheduleSlotDto>>
    {
        public Guid ExperienceId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
