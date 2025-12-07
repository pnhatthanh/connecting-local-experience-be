using BuildingBlocks.Application.CQRS.Query;
using User.Application.DTOs;

namespace User.Application.Handlers.Queries.GetBatchUserInfo
{
    public record GetBatchUserInfoQuery(
        List<Guid> UserIds
    ) : IQuery<List<UserInfoDto>>;
}
