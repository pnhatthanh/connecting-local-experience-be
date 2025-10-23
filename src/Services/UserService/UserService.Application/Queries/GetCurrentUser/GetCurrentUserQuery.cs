using BuildingBlocks.Application.CQRS.Query;
using UserService.Application.DTOs;

namespace UserService.Application.Queries.GetCurrentUser
{
    public class GetCurrentUserQuery : IQuery<UserProfileResponse>
    {
    }
}
