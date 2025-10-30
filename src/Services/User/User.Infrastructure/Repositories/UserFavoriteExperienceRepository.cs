using BuildingBlocks.EntityFramework;
using User.Domain.Entities;
using User.Domain.Repositories;
using User.Infrastructure.Data;

namespace User.Infrastructure.Repositories
{
    public class UserFavoriteExperienceRepository(UserDbContext context)
        : BaseRepository<UserFavoriteExperienceEntity>(context), IUserFavoriteExperienceRepository
    {}
}
