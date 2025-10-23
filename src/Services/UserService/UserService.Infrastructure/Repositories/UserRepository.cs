using BuildingBlocks.EntityFramework;
using UserService.Domain.Entities;
using UserService.Domain.Repositories;
using UserService.Domain.Specifications;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Repositories
{
    public class ProfileRepository : BaseRepository<ProfileEntity>, IProfileRepository
    {
        public ProfileRepository(UserDbContext context) : base(context)
        {
        }

        public async Task<ProfileEntity?> GetByAccountIdAsync(Guid accountId)
        {
            var spec = new ProfileByAccountIdSpecification(accountId);
            return await GetAnyAsync(spec);
        }
    }
}
