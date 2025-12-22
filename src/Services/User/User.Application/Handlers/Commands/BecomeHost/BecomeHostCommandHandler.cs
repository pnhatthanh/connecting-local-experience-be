using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Application.EventBus.Abstractions;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using MapsterMapper;
using User.Application.DTOs;
using User.Application.Events;
using User.Application.Interfaces;
using User.Domain.Entities;
using User.Domain.Enums;
using User.Domain.Repositories;
using User.Domain.Specifications;

namespace User.Application.Handlers.Commands.BecomeHost
{
    public class BecomeHostCommandHandler : ICommandHandler<BecomeHostCommand, HostProfileDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventBus _eventBus;
        private readonly IPhotoService _photoService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public BecomeHostCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork,
            IPhotoService photoService, IEventBus eventBus, ICurrentUserService currentUserService, IMapper mapper)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _eventBus = eventBus;
            _photoService = photoService;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<HostProfileDto> Handle(BecomeHostCommand request, CancellationToken cancellationToken)
        {
            var userSpec = new UserByIdSpecification(_currentUserService.UserId);
            var user = await _userRepository.GetBySpecAsync(userSpec, user => user.HostProfile!)
                ?? throw new BadRequestException($"User with Id {_currentUserService.UserId} not found");
            user.FullName = request.FullName;
            user.PhoneNumber = request.PhoneNumber;
            user.DateOfBirth = request.DateOfBirth;
            user.Gender = Enum.Parse<Gender>(request.Gender, true);
            user.Country = request.Country;
            if (request.Avatar != null)
            {
                if (!string.IsNullOrEmpty(user.AvatarUrl))
                    await _photoService.DeleteImageAsync(user.AvatarUrl);
                user.AvatarUrl = await _photoService.UploadImageAsync(request.Avatar, "users/avatars");
            }
            if (user.HostProfile != null)
                throw new BadRequestException("User already has a host profile");

            var hostProfile = new HostProfileEntity
            {
                UserId = _currentUserService.UserId,
                Bio = request.Bio,
                SpokenLanguages = request.SpokenLanguages,
                Location = request.Location,
                DocumentUrl = await _photoService.UploadImageAsync(request.Document!, "users/documents"),
                VerifyStatus = VerifyStatus.Pending,
                IsVerified = false,
                Work = request.Work,
                Education = request.Education,
                FunFact = request.FunFact,
                TopicsOfInterest = request.TopicsOfInterest,
                DesiredHostingStyle = request.DesiredHostingStyle,
                ResponseTime = string.IsNullOrEmpty(request.ResponseTime) ? null : Enum.Parse<ResponseTime>(request.ResponseTime, true),
                FacebookUrl = request.FacebookUrl,
                InstagramUrl = request.InstagramUrl,
                LinkedInUrl = request.LinkedInUrl,
                CreatedAt = DateTime.UtcNow
            };
            
            user.HostProfile = hostProfile;
            _userRepository.Update(user);
            await _unitOfWork.SaveChangeAsync();
            var hostProfileSubmittedEvent = new HostProfileSubmittedEvent(
                hostProfile.Id,
                hostProfile.UserId,
                user.Email,
                user.FullName,
                hostProfile.DocumentUrl
            );
            await _eventBus.PublishAsync(hostProfileSubmittedEvent, cancellationToken);
            return _mapper.Map<HostProfileDto>(user);
        }
    }
}
