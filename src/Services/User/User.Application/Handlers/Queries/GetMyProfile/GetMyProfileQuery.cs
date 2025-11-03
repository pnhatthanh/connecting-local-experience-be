using BuildingBlocks.Application.CQRS.Query;
using User.Application.DTOs;

namespace User.Application.Handlers.Queries.GetMyProfile
{
    public class GetMyProfileQuery : IQuery<UserDto>
    {
    }
}
