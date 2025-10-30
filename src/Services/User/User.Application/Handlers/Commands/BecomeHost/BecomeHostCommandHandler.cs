using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Application.EventBus.Abstractions;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using Microsoft.Extensions.Logging;
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
        private readonly IHostProfileRepository _hostProfileRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventBus _eventBus;
        private readonly IPhotoService _photoService;
        private readonly ILogger<BecomeHostCommandHandler> _logger;

        public BecomeHostCommandHandler(
            IUserRepository userRepository,
            IHostProfileRepository hostProfileRepository,
            IUnitOfWork unitOfWork,
            IEventBus eventBus,
            IPhotoService photoService,
            ILogger<BecomeHostCommandHandler> logger)
        {
            _userRepository = userRepository;
            _hostProfileRepository = hostProfileRepository;
            _unitOfWork = unitOfWork;
            _eventBus = eventBus;
            _photoService = photoService;
            _logger = logger;
        }

        public async Task<HostProfileDto> Handle(BecomeHostCommand request, CancellationToken cancellationToken)
        {
            var userSpec = new UserByIdSpecification(request.UserId);
            var user = await _userRepository.GetBySpecAsync(userSpec)
                ?? throw new NotFoundException($"User with Id {request.UserId} not found");

            if (user.HostProfile != null)
                throw new BadRequestException("User already has a host profile");

            string? documentUrl = null;
            if (request.Document != null)
            {
                documentUrl = await _photoService.UploadImageAsync(request.Document, "users/documents");
            }

            var hostProfile = new HostProfileEntity
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Bio = request.Bio,
                SpokenLanguages = request.SpokenLanguages ?? [],
                Location = request.Location,
                DocumentUrl = documentUrl,
                VerifyStatus = VerifyStatus.Pending,
                IsVerified = false,
                ResponseRate = 0,
                TotalExperiences = 0,
                CreatedAt = DateTime.UtcNow
            };

            await _hostProfileRepository.AddAsync(hostProfile);
            await _unitOfWork.SaveChangeAsync();

            _logger.LogInformation("Host profile created for user {UserId}", request.UserId);

            var hostProfileSubmittedEvent = new HostProfileSubmittedEvent(
                hostProfile.Id,
                hostProfile.UserId,
                user.Email,
                user.FullName,
                hostProfile.DocumentUrl
            );

            await _eventBus.PublishAsync(hostProfileSubmittedEvent, cancellationToken);

            return new HostProfileDto
            {
                Id = hostProfile.Id,
                UserId = hostProfile.UserId,
                Bio = hostProfile.Bio,
                SpokenLanguages = hostProfile.SpokenLanguages,
                Location = hostProfile.Location,
                IsVerified = hostProfile.IsVerified,
                VerifyStatus = hostProfile.VerifyStatus,
                DocumentUrl = hostProfile.DocumentUrl,
                ResponseRate = hostProfile.ResponseRate,
                ResponseTime = hostProfile.ResponseTime,
                TotalExperiences = hostProfile.TotalExperiences,
                RatingAvg = hostProfile.RatingAvg,
                CreatedAt = hostProfile.CreatedAt,
                UpdatedAt = hostProfile.UpdatedAt
            };
        }
    }
}
