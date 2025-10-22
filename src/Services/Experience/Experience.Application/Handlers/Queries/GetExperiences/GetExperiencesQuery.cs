using BuildingBlocks.Application.Dtos;
using Experience.Application.Dtos;
using MediatR;

namespace Experience.Application.Handlers.Queries.GetExperiences
{
    public class GetExperiencesQuery : IRequest<PaginationResult<ExperienceDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Category { get; set; }
        public string? Status { get; set; }
    }
}
