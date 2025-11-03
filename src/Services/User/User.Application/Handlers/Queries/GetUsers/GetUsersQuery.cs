using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Application.Dtos;
using User.Application.DTOs;

namespace User.Application.Handlers.Queries.GetUsers
{
    public class GetUsersQuery : IQuery<PaginationResult<UserSummaryDto>>
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
    }
}
