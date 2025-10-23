using BuildingBlocks.Domain.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Domain.Repositories
{
    public interface IProfileRepository : IBaseRepository<ProfileEntity>
    {
        Task<ProfileEntity?> GetByAccountIdAsync(Guid accountId);
    }
}
