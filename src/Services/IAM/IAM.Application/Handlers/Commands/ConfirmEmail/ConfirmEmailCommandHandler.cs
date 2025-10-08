using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using IAM.Domain.Repositories;
using IAM.Domain.Specifications;

namespace IAM.Application.Handlers.Commands.ConfirmEmailCommand
{
    public class ConfirmEmailCommandHandler : ICommandHandler<ConfirmEmailCommand, bool>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmEmailCommandHandler(
            IAccountRepository accountRepository,
            IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var emailSpec = new AccountEmailSpecification(request.Email.ToLowerInvariant());
            var account = await _accountRepository.GetAnyAsync(emailSpec)
                ?? throw new BadRequestException("Invalid email or token");

            if (account.IsEmailConfirmed)
            {
                throw new BadRequestException("Email is already confirmed");
            }

            if (string.IsNullOrEmpty(account.EmailConfirmationToken) || 
                account.EmailConfirmationToken != request.Token)
            {
                throw new BadRequestException("Invalid email or token");
            }

            account.IsEmailConfirmed = true;
            account.EmailConfirmationToken = null; 

            _accountRepository.Update(account);
            await _unitOfWork.SaveChangeAsync();

            return true;
        }
    }
}
