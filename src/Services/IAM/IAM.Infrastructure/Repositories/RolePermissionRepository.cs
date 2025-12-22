using BuildingBlocks.EntityFramework;
using IAM.Domain.Entities;
using IAM.Domain.Repositories;
using IAM.Infrastructure.Data;

namespace IAM.Infrastructure.Repositories
{
    public class RolePermissionRepository : BaseRepository<RolePermissionEntity>, IRolePermissionRepository
    {
        public RolePermissionRepository(IAMDbContext context) : base(context)
        {
        }
    }
}
