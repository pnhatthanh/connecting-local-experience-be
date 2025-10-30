using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Domain.Exceptions;
using User.Application.DTOs;
using User.Domain.Repositories;
using User.Domain.Specifications;

namespace User.Application.Handlers.Queries.GetUserById
{
    public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDto?>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var userSpec = new UserByIdSpecification(request.UserId);
            var user = await _userRepository.GetBySpecAsync(userSpec)
                ?? throw new BadRequestException($"User with Id {request.UserId} not found");

            var dto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                AvatarUrl = user.AvatarUrl,
                Country = user.Country,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            if (user.HostProfile != null)
            {
                dto.HostProfile = new HostProfileDto
                {
                    Id = user.HostProfile.Id,
                    UserId = user.HostProfile.UserId,
                    Bio = user.HostProfile.Bio,
                    SpokenLanguages = user.HostProfile.SpokenLanguages,
                    Location = user.HostProfile.Location,
                    IsVerified = user.HostProfile.IsVerified,
                    VerifyStatus = user.HostProfile.VerifyStatus,
                    DocumentUrl = user.HostProfile.DocumentUrl,
                    VerifyReason = user.HostProfile.VerifyReason,
                    VerifiedAt = user.HostProfile.VerifiedAt,
                    VerifiedBy = user.HostProfile.VerifiedBy,
                    ResponseRate = user.HostProfile.ResponseRate,
                    ResponseTime = user.HostProfile.ResponseTime,
                    TotalExperiences = user.HostProfile.TotalExperiences,
                    RatingAvg = user.HostProfile.RatingAvg,
                    CreatedAt = user.HostProfile.CreatedAt,
                    UpdatedAt = user.HostProfile.UpdatedAt
                };
            }

            return dto;
        }
    }
}
