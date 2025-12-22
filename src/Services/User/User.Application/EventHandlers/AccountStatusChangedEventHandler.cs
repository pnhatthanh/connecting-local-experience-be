using BuildingBlocks.Application.EventBus.Abstractions;
using BuildingBlocks.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using User.Application.Events;
using User.Domain.Enums;
using User.Domain.Repositories;
using User.Domain.Specifications;

namespace User.Application.EventHandlers
{
    public class AccountStatusChangedEventHandler : IIntegrationEventHandler<AccountStatusChangedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountStatusChangedEventHandler> _logger;

        public AccountStatusChangedEventHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ILogger<AccountStatusChangedEventHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task HandleAsync(AccountStatusChangedEvent @event, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation(
                    "Handling AccountStatusChangedEvent for AccountId: {AccountId}, IsActive: {IsActive}", 
                    @event.AccountId, 
                    @event.IsActive);

                var userSpec = new UserByIdSpecification(@event.AccountId);
                var user = await _userRepository.GetBySpecAsync(userSpec);

                if (user == null)
                {
                    _logger.LogWarning("User not found for AccountId: {AccountId}", @event.AccountId);
                    return;
                }

                var newStatus = @event.IsActive ? UserStatus.Active : UserStatus.Inactive;

                if (user.Status == newStatus)
                {
                    _logger.LogInformation(
                        "User already has status {Status} for AccountId: {AccountId}", 
                        newStatus, 
                        @event.AccountId);
                    return;
                }

                user.Status = newStatus;
                user.UpdatedAt = DateTime.UtcNow;

                _userRepository.Update(user);
                await _unitOfWork.SaveChangeAsync();

                _logger.LogInformation(
                    "User status updated to {Status} for AccountId: {AccountId}", 
                    newStatus, 
                    @event.AccountId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, 
                    "Error handling AccountStatusChangedEvent for AccountId: {AccountId}", 
                    @event.AccountId);
                throw;
            }
        }
    }
}
