using BuildingBlocks.Application.CQRS.Command;
using UserService.Domain.Repositories;

namespace UserService.Application.Commands.Admin.ActivateUser
{
    public class ActivateUserCommandHandler : ICommandHandler<ActivateUserCommand, bool>
    {
        private readonly IAccountRepository _accountRepository;
        public ActivateUserCommandHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public async Task<bool> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
        {
            return await _accountRepository.ActivateAccountAsync(request.UserId);
        }
    }
}
