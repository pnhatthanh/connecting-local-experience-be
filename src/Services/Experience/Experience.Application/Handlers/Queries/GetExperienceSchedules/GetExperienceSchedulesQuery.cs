using Experience.Application.Dtos;
using MediatR;

namespace Experience.Application.Handlers.Queries.GetExperienceSchedules
{
    public class GetExperienceSchedulesQuery : IRequest<List<ExperienceScheduleDto>>
    {
        public Guid ExperienceId { get; set; }
    }
}
