using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Application.EventBus.Abstractions;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using IAM.Application.DTOs;
using IAM.Application.Events;
using IAM.Application.Interfaces;
using IAM.Domain.Entities;
using IAM.Domain.Enums;
using IAM.Domain.Repositories;
using IAM.Domain.Specifications;

namespace IAM.Application.Handlers.Commands.RegisterCommand
{
    public class RegisterCommandHandler : ICommandHandler<RegisterCommand, RegisterResponse>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventBus _eventBus;

        public RegisterCommandHandler(
            IAccountRepository accountRepository,
            IRoleRepository roleRepository,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork,
            IEventBus eventBus)
        {
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
            _eventBus = eventBus;
        }

        public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var emailExistsSpec = new AccountEmailSpecification(request.Email.ToLowerInvariant());
            if (await _accountRepository.CheckExistsAsync(emailExistsSpec))
            {
                throw new DuplicatedException("Email is already registered");
            }
            
            var userRoleSpec = new RoleByNameSpecification(AccountRole.User);
            var userRole = await _roleRepository.GetBySpecAsync(userRoleSpec) 
                ?? throw new NotFoundException("Default User role not found");
            
            var confirmationToken = Guid.NewGuid().ToString("N");

            var account = new AccountEntity
            {
                FullName = request.FullName,
                Email = request.Email.ToLowerInvariant(),
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                RoleId = userRole.Id,
                IsEmailConfirmed = false,
                EmailConfirmationToken = confirmationToken,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _accountRepository.AddAsync(account);
            await _unitOfWork.SaveChangeAsync();

            var emailConfirmationEvent = new EmailConfirmationRequestedEvent(
                account.Id,
                account.FullName,
                account.Email,
                confirmationToken
            );

            await _eventBus.PublishAsync(emailConfirmationEvent, cancellationToken);

            return new RegisterResponse
            {
                AccountId = account.Id,
                Email = account.Email,
                FullName = account.FullName
            };
        }
    }
}
