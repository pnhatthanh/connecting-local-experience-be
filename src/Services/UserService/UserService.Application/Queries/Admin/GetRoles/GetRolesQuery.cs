using BuildingBlocks.Application.CQRS.Query;
using UserService.Application.DTOs.Admin;

namespace UserService.Application.Queries.Admin.GetRoles
{
    public class GetRolesQuery : IQuery<List<RoleDto>>
    {
    }
}
