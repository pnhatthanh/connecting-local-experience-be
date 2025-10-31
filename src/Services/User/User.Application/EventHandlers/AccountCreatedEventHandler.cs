using BuildingBlocks.Application.EventBus.Abstractions;
using BuildingBlocks.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using User.Application.Events;
using User.Domain.Entities;
using User.Domain.Enums;
using User.Domain.Repositories;
using User.Domain.Specifications;

namespace User.Application.EventHandlers
{
    public class AccountCreatedEventHandler : IIntegrationEventHandler<AccountCreatedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountCreatedEventHandler> _logger;

        public AccountCreatedEventHandler(IUserRepository userRepository,
            IUnitOfWork unitOfWork, ILogger<AccountCreatedEventHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task HandleAsync(AccountCreatedEvent @event, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Handling AccountCreatedEvent for AccountId: {AccountId}", @event.AccountId);
                
                var existingUser = await _userRepository.CheckExistsAsync(new UserByIdSpecification(@event.AccountId));
                if (existingUser)
                {
                    _logger.LogWarning("User profile already exists for AccountId: {AccountId}", @event.AccountId);
                    return;
                }
                var userEntity = new UserEntity
                {
                    Id = @event.AccountId, 
                    Email = @event.Email,
                    FullName = @event.FullName,
                    Role = UserRole.User,
                    CreatedAt = DateTime.UtcNow
                };

                await _userRepository.AddAsync(userEntity);
                await _unitOfWork.SaveChangeAsync();

                _logger.LogInformation("User profile created successfully for AccountId: {AccountId}, UserId: {UserId}",
                    @event.AccountId, userEntity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling AccountCreatedEvent for AccountId: {AccountId}", @event.AccountId);
            }
        }
    }
}
