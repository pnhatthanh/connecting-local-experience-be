using Experience.Application.Dtos;
using MediatR;

namespace Experience.Application.Handlers.Queries.GetExperienceAvailability
{
    public class GetExperienceAvailabilityQuery : IRequest<List<ExperienceScheduleSlotDto>>
    {
        public Guid ExperienceId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
