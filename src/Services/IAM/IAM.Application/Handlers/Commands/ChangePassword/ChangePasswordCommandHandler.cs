using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using IAM.Application.Interfaces;
using IAM.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace IAM.Application.Handlers.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand, bool>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ChangePasswordCommandHandler> _logger;

        public ChangePasswordCommandHandler(IAccountRepository accountRepository, ICurrentUserService currentUserService,
            IPasswordHasher passwordHasher, IUnitOfWork unitOfWork,
            ILogger<ChangePasswordCommandHandler> logger)
        {
            _accountRepository = accountRepository;
            _currentUserService = currentUserService;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var user = await _accountRepository.GetByIdAsync(userId) 
                ?? throw new NotFoundException("User not found");

            // Verify current password
            if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
                throw new BadRequestException("Current password is incorrect");

            user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            _accountRepository.Update(user);
            await _unitOfWork.SaveChangeAsync();
            _logger.LogInformation("User {UserId} successfully changed password", userId);
            return true;
        }
    }
}
