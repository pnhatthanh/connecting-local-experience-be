using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using UserService.Application.DTOs;
using UserService.Domain.Repositories;

namespace UserService.Application.Queries.GetCurrentUser
{
    public class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, UserProfileResponse>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetCurrentUserQueryHandler(
            IProfileRepository profileRepository,
            ICurrentUserService currentUserService)
        {
            _profileRepository = profileRepository;
            _currentUserService = currentUserService;
        }

        public async Task<UserProfileResponse> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            // Temporarily disabled authentication check for testing
            // if (!_currentUserService.IsAuthenticated)
            // {
            //     throw new UnAuthorizedException("User is not authenticated");
            // }

            var accountId = Guid.Parse(_currentUserService.UserId);
            var profile = await _profileRepository.GetByAccountIdAsync(accountId)
                ?? throw new NotFoundException("User profile not found");

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
