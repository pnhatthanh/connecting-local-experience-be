using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using IAM.Domain.Repositories;
using IAM.Domain.Specifications;

namespace IAM.Application.Handlers.Commands.Logout
{
    public class LogoutCommandHandler : ICommandHandler<LogoutCommand, bool>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository,
            ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var spec = new RefreshTokenByTokenSpecification(userId, request.RefreshToken);
            var token = await _refreshTokenRepository.GetBySpecAsync(spec)
                ?? throw new BadRequestException("Invalid refresh token.");

            _refreshTokenRepository.Delete(token);
            await _unitOfWork.SaveChangeAsync();
            return true;
        }
    }
}
