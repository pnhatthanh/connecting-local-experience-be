using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Application.Dtos;
using UserService.Application.DTOs.Admin;

namespace UserService.Application.Queries.Admin.ListUsers
{
    public class ListUsersQuery : IQuery<PaginationResult<AdminUserListItem>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public bool IsAscending { get; set; } = true;
        public string? Keyword { get; set; }
        public bool? IsActive { get; set; }
        
        /// <summary>
        /// Tên role để lọc. Các giá trị hợp lệ: Admin, User, Host
        /// </summary>
        public string? RoleName { get; set; }
    }
}
