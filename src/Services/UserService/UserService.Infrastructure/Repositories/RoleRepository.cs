using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Repositories;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly UserDbContext _context;

        public RoleRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<List<RoleEntity>> GetAllAsync()
        {
            return await _context.Roles.AsNoTracking().ToListAsync();
        }

        public async Task<RoleEntity?> GetByIdAsync(Guid roleId)
        {
            return await _context.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == roleId);
        }
    }
}
