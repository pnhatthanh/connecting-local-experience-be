using BuildingBlocks.EntityFramework;
using IAM.Domain.Entities;
using IAM.Domain.Repositories;
using IAM.Infrastructure.Data;

namespace IAM.Infrastructure.Repositories
{
    public class PermissionRepository : BaseRepository<PermissionEntity>, IPermissionRepository
    {
        public PermissionRepository(IAMDbContext context) : base(context)
        {
        }
    }
}
