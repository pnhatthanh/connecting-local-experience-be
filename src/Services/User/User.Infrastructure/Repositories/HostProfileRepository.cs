using BuildingBlocks.EntityFramework;
using User.Domain.Entities;
using User.Domain.Repositories;
using User.Infrastructure.Data;

namespace User.Infrastructure.Repositories
{
    public class HostProfileRepository(UserDbContext context) 
        : BaseRepository<HostProfileEntity>(context), IHostProfileRepository
    {}
}
