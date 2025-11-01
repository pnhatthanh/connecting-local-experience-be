using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Application.EventBus.Abstractions;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using IAM.Application.Events;
using IAM.Domain.Repositories;
using IAM.Domain.Specifications;

namespace IAM.Application.Handlers.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand, bool>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventBus _eventBus;

        public ForgotPasswordCommandHandler(
            IAccountRepository accountRepository,
            IUnitOfWork unitOfWork,
            IEventBus eventBus)
        {
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
            _eventBus = eventBus;
        }

        public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var emailSpec = new AccountEmailSpecification(request.Email.ToLowerInvariant());
            var account = await _accountRepository.GetBySpecAsync(emailSpec)
                ?? throw new BadRequestException("Email not found");
            
            var random = new Random();
            var resetToken = random.Next(100000, 999999).ToString(); 
            account.PasswordResetToken = resetToken;
            account.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1); 
            _accountRepository.Update(account);
            await _unitOfWork.SaveChangeAsync();

            var passwordResetEvent = new PasswordResetRequestedEvent(
                account.Id,
                account.FullName,
                account.Email,
                resetToken
            );

            await _eventBus.PublishAsync(passwordResetEvent, cancellationToken);

            return true;
        }
    }
}
