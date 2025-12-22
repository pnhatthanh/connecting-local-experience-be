using BuildingBlocks.Application.Dtos;
using Experience.Application.Dtos;
using MediatR;

namespace Experience.Application.Handlers.Queries.GetExperiences
{
    public class GetExperiencesQuery : IRequest<PaginationResult<ExperienceSummaryDto>>
    {
        public string? SearchTerm { get; set; }
        public Guid? HostId { get; set; }
        public List<Guid>? CategoryIds { get; set; }
        public int? MinDuration { get; set; }
        public int? MaxDuration { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public List<string>? Languages { get; set; }
        public string? SortBy { get; set; }
        public bool IsAscending { get; set; } = true;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Status { get; set; } 
    }
}
