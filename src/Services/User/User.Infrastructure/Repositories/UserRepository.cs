using BuildingBlocks.EntityFramework;
using Microsoft.EntityFrameworkCore;
using User.Domain.Entities;
using User.Domain.Repositories;
using User.Infrastructure.Data;

namespace User.Infrastructure.Repositories
{
    public class UserRepository(UserDbContext context) 
        : BaseRepository<UserEntity>(context), IUserRepository
    {
        public async Task<List<UserEntity>> GetByIdsAsync(List<Guid> userIds)
        {
            return await context.Set<UserEntity>()
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();
        }
    }
}
