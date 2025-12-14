using BuildingBlocks.Application.CQRS.Query;
using User.Application.DTOs;
using User.Domain.Repositories;
using User.Domain.Specifications;

namespace User.Application.Handlers.Queries.GetBatchUserInfo
{
    public class GetBatchUserInfoQueryHandler : IQueryHandler<GetBatchUserInfoQuery, List<UserInfoDto>>
    {
        private readonly IUserRepository _userRepository;

        public GetBatchUserInfoQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserInfoDto>> Handle(GetBatchUserInfoQuery request, CancellationToken cancellationToken)
        {
            if (!request.UserIds.Any())
                return new List<UserInfoDto>();

            var usersByIdsSpec = new UsersByIdsSpecification(request.UserIds);
            var users = await _userRepository.GetAllAsync(usersByIdsSpec);

            return [.. users.Select(u => new UserInfoDto(u.Id, u.FullName, u.AvatarUrl))];
        }
    }
}
