using BuildingBlocks.Application.CQRS.Command;
using UserService.Domain.Repositories;

namespace UserService.Application.Commands.Admin.DeactivateUser
{
    public class DeactivateUserCommandHandler : ICommandHandler<DeactivateUserCommand, bool>
    {
        private readonly IAccountRepository _accountRepository;
        public DeactivateUserCommandHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public async Task<bool> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
        {
            var isActive = await _accountRepository.IsAccountActiveAsync(request.UserId);
            if (!isActive)
            {
                return false;
            }
            return await _accountRepository.DeactivateAccountAsync(request.UserId);
        }
    }
}
