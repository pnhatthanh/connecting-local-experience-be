using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using UserService.Application.DTOs;
using UserService.Domain.Repositories;

namespace UserService.Application.Commands.UpdateUserProfile
{
    public class UpdateUserProfileCommandHandler : ICommandHandler<UpdateUserProfileCommand, UserProfileResponse>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserProfileCommandHandler(
            IProfileRepository profileRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _profileRepository = profileRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<UserProfileResponse> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            // Temporarily disabled authentication check for testing
            // if (!_currentUserService.IsAuthenticated)
            // {
            //     throw new UnAuthorizedException("User is not authenticated");
            // }

            var accountId = Guid.Parse(_currentUserService.UserId);
            var profile = await _profileRepository.GetByAccountIdAsync(accountId)
                ?? throw new NotFoundException("User profile not found");

            // Update fields only if provided
            if (!string.IsNullOrWhiteSpace(request.FullName))
                profile.FullName = request.FullName;

            if (request.PhoneNumber != null)
                profile.PhoneNumber = request.PhoneNumber;

            if (request.Nationality != null)
                profile.Nationality = request.Nationality;

            if (request.AvatarUrl != null)
                profile.AvatarUrl = request.AvatarUrl;

            _profileRepository.Update(profile);
            await _unitOfWork.SaveChangeAsync();

            return new UserProfileResponse
            {
                Id = profile.Id,
                AccountId = profile.AccountId,
                FullName = profile.FullName,
                PhoneNumber = profile.PhoneNumber,
                Nationality = profile.Nationality,
                AvatarUrl = profile.AvatarUrl,
                CreatedAt = profile.CreatedAt,
                UpdatedAt = profile.UpdatedAt
            };
        }
    }
}
