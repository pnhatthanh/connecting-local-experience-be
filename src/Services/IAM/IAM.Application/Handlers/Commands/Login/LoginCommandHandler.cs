using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using IAM.Application.DTOs;
using IAM.Application.Interfaces;
using IAM.Domain.Entities;
using IAM.Domain.Repositories;
using IAM.Domain.Specifications;
using MediatR;

namespace IAM.Application.Handlers.Commands.Login
{
    public class LoginCommandHandler : ICommandHandler<LoginCommand, TokenResponse>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUnitOfWork _unitOfWork;

        public LoginCommandHandler(
            IAccountRepository accountRepository, 
            IRefreshTokenRepository refreshTokenRepository,
            IRolePermissionRepository rolePermissionRepository,
            IPasswordHasher passwordHasher, 
            IJwtTokenService jwtTokenService, 
            IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<TokenResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var emailSpec = new AccountEmailSpecification(request.Email.ToLowerInvariant());
            var account = await _accountRepository.GetBySpecAsync(emailSpec, account => account.Role)
                ?? throw new BadRequestException("Invalid email or password");
            if (!_passwordHasher.VerifyPassword(request.Password, account.PasswordHash))
                throw new BadRequestException("Invalid email or password");

            if (!account.IsActive)
                throw new ForbiddenException("Account is deactivated");

            if (!account.IsEmailConfirmed)
                throw new ForbiddenException("Email is not confirmed");
            
            // Load permissions based on role using Specification Pattern
            var rolePermissionSpec = new RolePermissionByRoleIdSpecification(account.RoleId);
            var rolePermissions = await _rolePermissionRepository.GetAllAsync(
                rolePermissionSpec, 
                rp => rp.Permission  // Include Permission entity
            );
            var permissionCodes = rolePermissions.Select(rp => rp.Permission.PermissionCode).ToList();
            
            var accessToken = _jwtTokenService.GenerateAccessToken(account, permissionCodes);
            var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();
            var refreshToken = new RefreshTokenEntity
            {
                AccountId = account.Id,
                Token = refreshTokenValue,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };
            await _refreshTokenRepository.AddAsync(refreshToken);
            account.LastLoginAt = DateTime.UtcNow;
            _accountRepository.Update(account);
            await _unitOfWork.SaveChangeAsync();

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue
            };
        }
    }
}
