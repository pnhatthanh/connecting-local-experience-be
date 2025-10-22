using Experience.Application.Dtos;
using MediatR;

namespace Experience.Application.Handlers.Queries.GetExperience
{
    public class GetExperienceQuery : IRequest<ExperienceDto?>
    {
        public Guid ExperienceId { get; set; }
    }
}
