using BuildingBlocks.EntityFramework;
using User.Domain.Entities;
using User.Domain.Repositories;
using User.Infrastructure.Data;

namespace User.Infrastructure.Repositories
{
    public class WishlistExperienceRepository : BaseRepository<WishlistExperienceEntity>, IWishlistExperienceRepository
    {
        public WishlistExperienceRepository(UserDbContext context) : base(context)
        {
        }
    }
}
