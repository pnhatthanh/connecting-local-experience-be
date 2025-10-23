using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Domain.Exceptions;
using UserService.Application.DTOs.Admin;
using UserService.Domain.Repositories;

namespace UserService.Application.Queries.Admin.GetUserDetail
{
    public class GetUserDetailQueryHandler : IQueryHandler<GetUserDetailQuery, AdminUserDetailResponse>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IRoleRepository _roleRepository;

        public GetUserDetailQueryHandler(IAccountRepository accountRepository, IProfileRepository profileRepository, IRoleRepository roleRepository)
        {
            _accountRepository = accountRepository;
            _profileRepository = profileRepository;
            _roleRepository = roleRepository;
        }

        public async Task<AdminUserDetailResponse> Handle(GetUserDetailQuery request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(request.UserId)
                ?? throw new NotFoundException($"Account {request.UserId} not found");

            var profile = await _profileRepository.GetByAccountIdAsync(request.UserId);
            var role = account.RoleId.HasValue ? await _roleRepository.GetByIdAsync(account.RoleId.Value) : null;

            return new AdminUserDetailResponse
            {
                AccountId = account.IdAccount,
                Email = account.Email,
                IsActive = account.IsActive,
                RoleId = account.RoleId,
                RoleName = role?.Name.ToString(),
                FullName = profile?.FullName ?? account.FullName,
                PhoneNumber = profile?.PhoneNumber,
                Nationality = profile?.Nationality,
                AvatarUrl = profile?.AvatarUrl,
                CreatedAt = account.CreatedAt,
                UpdatedAt = account.UpdatedAt
            };
        }
    }
}
