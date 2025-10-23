using BuildingBlocks.Application.CQRS.Query;
using UserService.Application.DTOs.Admin;

namespace UserService.Application.Queries.Admin.GetUserDetail
{
    public class GetUserDetailQuery : IQuery<AdminUserDetailResponse>
    {
        public Guid UserId { get; set; }
        public GetUserDetailQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
