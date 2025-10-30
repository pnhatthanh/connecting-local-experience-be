using BuildingBlocks.Application.CQRS.Query;
using User.Application.DTOs;

namespace User.Application.Handlers.Queries.GetUserById
{
    public class GetUserByIdQuery : IQuery<UserDto?>
    {
        public Guid UserId { get; set; }
    }
}
