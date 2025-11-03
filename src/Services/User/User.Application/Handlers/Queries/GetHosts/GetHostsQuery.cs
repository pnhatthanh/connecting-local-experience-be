using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Application.Dtos;
using User.Application.DTOs;

namespace User.Application.Handlers.Queries.GetHosts
{
    public class GetHostsQuery : IQuery<PaginationResult<HostSummaryDto>>
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? Status { get; set; }
    }
}
