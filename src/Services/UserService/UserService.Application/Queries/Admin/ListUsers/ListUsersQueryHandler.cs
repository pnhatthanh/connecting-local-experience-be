using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Application.Dtos;
using UserService.Application.DTOs.Admin;
using UserService.Domain.Repositories;

namespace UserService.Application.Queries.Admin.ListUsers
{
    public class ListUsersQueryHandler : IQueryHandler<ListUsersQuery, PaginationResult<AdminUserListItem>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IRoleRepository _roleRepository;

        public ListUsersQueryHandler(IAccountRepository accountRepository, IRoleRepository roleRepository)
        {
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
        }

        public async Task<PaginationResult<AdminUserListItem>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
        {
            var (items, total) = await _accountRepository.GetPagedAsync(request.PageNumber, request.PageSize, request.Keyword, request.IsActive, request.RoleName, request.SortBy, request.IsAscending);
            var roles = await _roleRepository.GetAllAsync();

            var data = items.Select(a =>
            {
                var role = roles.FirstOrDefault(r => r.Id == a.RoleId);
                return new AdminUserListItem
                {
                    AccountId = a.IdAccount,
                    Email = a.Email,
                    FullName = a.FullName ?? string.Empty,
                    IsActive = a.IsActive,
                    RoleId = a.RoleId,
                    RoleName = role?.Name.ToString(),
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                };
            }).ToList();

            return new PaginationResult<AdminUserListItem>
            {
                Data = data,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
