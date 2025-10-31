using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Application.EventBus.Abstractions;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using IAM.Application.Events;
using IAM.Domain.Repositories;
using IAM.Domain.Specifications;

namespace IAM.Application.Handlers.Commands.ConfirmEmail
{
    public class ConfirmEmailCommandHandler : ICommandHandler<ConfirmEmailCommand, bool>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventBus _eventBus;

        public ConfirmEmailCommandHandler(
            IAccountRepository accountRepository,
            IUnitOfWork unitOfWork,
            IEventBus eventBus)
        {
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
            _eventBus = eventBus;
        }

        public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var emailSpec = new AccountEmailSpecification(request.Email.ToLowerInvariant());
            var account = await _accountRepository.GetBySpecAsync(emailSpec)
                ?? throw new BadRequestException("Email is not exist");
            if (account.IsEmailConfirmed)
                throw new BadRequestException("Email is already confirmed");

            if (account.EmailConfirmationToken != request.Token)
                throw new BadRequestException("Invalid email or token");

            account.IsEmailConfirmed = true;
            account.EmailConfirmationToken = null; 

            _accountRepository.Update(account);
            await _unitOfWork.SaveChangeAsync();
            await _eventBus.PublishAsync(new AccountCreatedEvent(account.Id, account.FullName, account.Email, DateTime.UtcNow));

            return true;
        }
    }
}
