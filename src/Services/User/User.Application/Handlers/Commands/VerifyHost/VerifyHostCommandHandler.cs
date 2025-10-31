using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Application.EventBus.Abstractions;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using User.Application.DTOs;
using User.Application.Events;
using User.Domain.Enums;
using User.Domain.Repositories;
using User.Domain.Specifications;

namespace User.Application.Handlers.Commands.VerifyHost
{
    public class VerifyHostCommandHandler : ICommandHandler<VerifyHostCommand, HostProfileDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventBus _eventBus;
        private readonly ILogger<VerifyHostCommandHandler> _logger;
        private readonly IMapper _mapper;

        public VerifyHostCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork, 
            IEventBus eventBus, 
            ILogger<VerifyHostCommandHandler> logger,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _eventBus = eventBus;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<HostProfileDto> Handle(VerifyHostCommand request, CancellationToken cancellationToken)
        {
            var userSpec = new UserByIdSpecification(request.AccountId);
            var userProfile = await _userRepository.GetBySpecAsync(userSpec, user => user.HostProfile!)
                ?? throw new BadRequestException($"User profile with Id {request.AccountId} not found");
            if(userProfile.HostProfile == null)
                throw new BadRequestException($"Host profile for user Id {request.AccountId} not found");
            if (userProfile.HostProfile.VerifyStatus != VerifyStatus.Pending)
                throw new BadRequestException("Only pending host profiles can be verified");
            userProfile.HostProfile.VerifyStatus = Enum.Parse<VerifyStatus>(request.Status);
            userProfile.HostProfile.IsVerified = request.Status == VerifyStatus.Approved.ToString();
            userProfile.HostProfile.VerifyReason = request.Reason;
            userProfile.HostProfile.VerifiedAt = DateTime.UtcNow;
            userProfile.HostProfile.UpdatedAt = DateTime.UtcNow;
            if (userProfile.HostProfile.VerifyStatus == VerifyStatus.Approved)
            {
                userProfile.Role = UserRole.Host;
            }
            _userRepository.Update(userProfile);
            await _unitOfWork.SaveChangeAsync();

            _logger.LogInformation("Host profile for UserId {UserId} verified with status {Status}",
                request.AccountId, request.Status);

            var hostProfileVerifiedEvent = new HostProfileVerifiedEvent(
                userProfile.HostProfile.UserId,
                userProfile.HostProfile.VerifyStatus == VerifyStatus.Approved,
                request.Reason
            );

            await _eventBus.PublishAsync(hostProfileVerifiedEvent, cancellationToken);

            return _mapper.Map<HostProfileDto>(userProfile.HostProfile);
        }
    }
}
