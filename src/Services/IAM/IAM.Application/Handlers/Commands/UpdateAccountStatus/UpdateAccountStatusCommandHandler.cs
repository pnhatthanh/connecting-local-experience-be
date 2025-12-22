using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Application.EventBus.Abstractions;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using IAM.Application.Events;
using IAM.Domain.Repositories;

namespace IAM.Application.Handlers.Commands.UpdateAccountStatus
{
    public class UpdateAccountStatusCommandHandler : ICommandHandler<UpdateAccountStatusCommand, bool>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventBus _eventBus;

        public UpdateAccountStatusCommandHandler(
            IAccountRepository accountRepository,
            IUnitOfWork unitOfWork,
            IEventBus eventBus)
        {
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
            _eventBus = eventBus;
        }

        public async Task<bool> Handle(UpdateAccountStatusCommand request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(request.AccountId)
                ?? throw new NotFoundException("Account not found");

            if (account.IsActive == request.IsActive)
            {
                throw new BadRequestException($"Account is already {(request.IsActive ? "active" : "inactive")}");
            }

            account.IsActive = request.IsActive;
            account.UpdatedAt = DateTime.UtcNow;
            
            _accountRepository.Update(account);
            await _unitOfWork.SaveChangeAsync();

            // Publish event to update user status in User service
            var accountStatusChangedEvent = new AccountStatusChangedEvent(account.Id, request.IsActive);
            await _eventBus.PublishAsync(accountStatusChangedEvent, cancellationToken);

            return true;
        }
    }
}
