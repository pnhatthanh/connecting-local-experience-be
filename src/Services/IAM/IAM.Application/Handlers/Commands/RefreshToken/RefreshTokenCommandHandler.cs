using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using IAM.Application.DTOs;
using IAM.Application.Interfaces;
using IAM.Domain.Entities;
using IAM.Domain.Repositories;
using IAM.Domain.Specifications;

namespace IAM.Application.Handlers.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, TokenResponse>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUnitOfWork _unitOfWork;

        public RefreshTokenCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            IAccountRepository accountRepository,
            IRolePermissionRepository rolePermissionRepository,
            IJwtTokenService jwtTokenService,
            IUnitOfWork unitOfWork)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _accountRepository = accountRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _jwtTokenService = jwtTokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<TokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshTokenSpec = new RefreshTokenSpecification(request.RefreshToken);
            var refreshToken = await _refreshTokenRepository.GetBySpecAsync(refreshTokenSpec)
                ?? throw new UnAuthorizedException("Invalid refresh token");
            if (!refreshToken.IsActive)
                throw new UnAuthorizedException("Refresh token is expired or revoked");
            
            var accountSpec = new AccountByIdSpecification(refreshToken.AccountId);
            var account = await _accountRepository.GetBySpecAsync(accountSpec, account => account.Role)
                ?? throw new NotFoundException("Account not found");
            if (!account.IsActive)
                throw new ForbiddenException("Account is deactivated");
            if (!account.IsEmailConfirmed)
                throw new ForbiddenException("Email is not confirmed");

            var rolePermissionSpec = new RolePermissionByRoleIdSpecification(account.RoleId);
            var rolePermissions = await _rolePermissionRepository.GetAllAsync(
                rolePermissionSpec,
                rp => rp.Permission
            );
            var permissionCodes = rolePermissions.Select(rp => rp.Permission.PermissionCode).ToList();

            var newAccessToken = _jwtTokenService.GenerateAccessToken(account, permissionCodes);
            var newRefreshTokenValue = _jwtTokenService.GenerateRefreshToken();
            
            _refreshTokenRepository.Delete(refreshToken);
            var newRefreshToken = new RefreshTokenEntity
            {
                AccountId = account.Id,
                Token = newRefreshTokenValue,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };
            await _refreshTokenRepository.AddAsync(newRefreshToken);
            await _unitOfWork.SaveChangeAsync();
            return new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenValue
            };
        }
    }
}
