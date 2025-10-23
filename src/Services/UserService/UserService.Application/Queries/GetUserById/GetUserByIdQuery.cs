using BuildingBlocks.Application.CQRS.Query;
using UserService.Application.DTOs;

namespace UserService.Application.Queries.GetUserById
{
    public class GetUserByIdQuery : IQuery<UserProfileResponse>
    {
        public Guid UserId { get; set; }

        public GetUserByIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
