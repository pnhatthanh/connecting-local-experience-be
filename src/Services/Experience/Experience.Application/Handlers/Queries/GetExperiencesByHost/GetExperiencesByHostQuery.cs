using Experience.Application.Dtos;
using MediatR;

namespace Experience.Application.Handlers.Queries.GetExperiencesByHost
{
    public class GetExperiencesByHostQuery : IRequest<List<ExperienceDto>>
    {
        public Guid HostId { get; set; }
    }
}
