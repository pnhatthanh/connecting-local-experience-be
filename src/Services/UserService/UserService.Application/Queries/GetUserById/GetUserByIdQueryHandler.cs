using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Domain.Exceptions;
using UserService.Application.DTOs;
using UserService.Domain.Repositories;

namespace UserService.Application.Queries.GetUserById
{
    public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserProfileResponse>
    {
        private readonly IProfileRepository _profileRepository;

        public GetUserByIdQueryHandler(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<UserProfileResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(request.UserId)
                ?? throw new NotFoundException($"User profile with account ID {request.UserId} not found");

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
