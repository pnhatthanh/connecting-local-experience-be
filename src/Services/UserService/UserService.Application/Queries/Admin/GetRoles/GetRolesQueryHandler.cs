using BuildingBlocks.Application.CQRS.Query;
using UserService.Application.DTOs.Admin;
using UserService.Domain.Repositories;

namespace UserService.Application.Queries.Admin.GetRoles
{
    public class GetRolesQueryHandler : IQueryHandler<GetRolesQuery, List<RoleDto>>
    {
        private readonly IRoleRepository _roleRepository;

        public GetRolesQueryHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<List<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleRepository.GetAllAsync();
            
            return roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = (int)r.Name,
                NameDisplay = r.Name.ToString(),
                Description = r.Description
            }).ToList();
        }
    }
}
