using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using IAM.Application.Interfaces;
using IAM.Domain.Repositories;
using IAM.Domain.Specifications;

namespace IAM.Application.Handlers.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand, bool>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public ResetPasswordCommandHandler(
            IAccountRepository accountRepository,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var emailSpec = new AccountEmailSpecification(request.Email.ToLowerInvariant());
            var account = await _accountRepository.GetBySpecAsync(emailSpec)
                ?? throw new BadRequestException("Invalid email or token");
            if (string.IsNullOrEmpty(account.PasswordResetToken) || account.PasswordResetToken != request.Token)
                throw new BadRequestException("Invalid email or token");
            if (account.PasswordResetTokenExpiry == null || account.PasswordResetTokenExpiry < DateTime.UtcNow)
                throw new BadRequestException("Token has expired");
            account.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
            account.PasswordResetToken = null;
            account.PasswordResetTokenExpiry = null;

            _accountRepository.Update(account);
            await _unitOfWork.SaveChangeAsync();

            return true;
        }
    }
}
