using BuildingBlocks.Domain.Interfaces;
using User.Domain.Entities;

namespace User.Domain.Repositories
{
    public interface IUserRepository : IBaseRepository<UserEntity>
    {
        Task<List<UserEntity>> GetByIdsAsync(List<Guid> userIds);
    }
}
