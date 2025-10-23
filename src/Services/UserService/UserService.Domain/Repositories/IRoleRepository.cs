using UserService.Domain.Entities;

namespace UserService.Domain.Repositories
{
    public interface IRoleRepository
    {
        Task<List<RoleEntity>> GetAllAsync();
        Task<RoleEntity?> GetByIdAsync(Guid roleId);
    }
}
