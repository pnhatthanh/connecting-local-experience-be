using BuildingBlocks.Application.EventBus.Abstractions;
using BuildingBlocks.Domain.Interfaces;
using IAM.Application.Events;
using IAM.Domain.Enums;
using IAM.Domain.Repositories;
using IAM.Domain.Specifications;
using Microsoft.Extensions.Logging;
    
namespace IAM.Application.EventHandlers
{
    public class HostProfileVerifiedEventHandler : IIntegrationEventHandler<HostProfileVerifiedEvent>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HostProfileVerifiedEventHandler> _logger;

        public HostProfileVerifiedEventHandler(IAccountRepository accountRepository,
            IRoleRepository roleRepository, IUnitOfWork unitOfWork, ILogger<HostProfileVerifiedEventHandler> logger)
        {
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task HandleAsync(HostProfileVerifiedEvent @event, CancellationToken cancellationToken = default)
        {
            try
            {
                if(@event.IsApproved == false)
                {
                    _logger.LogInformation("HostProfileVerifiedEvent received for UserId: {UserId} but not approved.", @event.UserId);
                    return;
                }
                _logger.LogInformation("Handling HostProfileVerifiedEvent for UserId: {UserId}", @event.UserId);
                var spec = new AccountByIdSpecification(@event.UserId);
                var account = await _accountRepository.GetBySpecAsync(spec, account => account.Role);
                if (account == null)
                {
                    _logger.LogWarning("Account not found for UserId: {UserId}", @event.UserId);
                    return;
                }
                var hostRoleSpec = new RoleByNameSpecification(AccountRole.Host);
                var hostRole = await _roleRepository.GetBySpecAsync(hostRoleSpec);
                if (hostRole == null)
                {
                    _logger.LogWarning("Host role not found in the system.");
                    return;
                }
                account.RoleId = hostRole.Id;
                _accountRepository.Update(account);
                await _unitOfWork.SaveChangeAsync();

                _logger.LogInformation("Successfully handled update role for UserId: {UserId}", @event.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling HostProfileVerifiedEvent for UserId: {UserId}", @event.UserId);
            }
        }
    }
}
