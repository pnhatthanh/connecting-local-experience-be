using BuildingBlocks.EntityFramework;
using IAM.Domain.Entities;
using IAM.Domain.Repositories;
using IAM.Infrastructure.Data;

namespace IAM.Infrastructure.Repositories
{
    public class RefreshTokenRepository(IAMDbContext context) 
        : BaseRepository<RefreshTokenEntity>(context), IRefreshTokenRepository
    {
    }
}