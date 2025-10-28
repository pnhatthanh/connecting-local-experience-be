using BuildingBlocks.EntityFramework;
using IAM.Domain.Entities;
using IAM.Domain.Repositories;
using IAM.Infrastructure.Data;

namespace IAM.Infrastructure.Repositories;

public class RoleRepository : BaseRepository<RoleEntity>, IRoleRepository
{
    public RoleRepository(IAMDbContext context) : base(context)
    {
    }
}
