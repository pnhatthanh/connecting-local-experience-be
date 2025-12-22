using BuildingBlocks.EntityFramework;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<Guid>> GetFavoriteExperienceIdsByUserIdAsync(Guid userId, List<Guid> experienceIds)
        {
            return await _context.Set<WishlistExperienceEntity>()
                .Where(we => experienceIds.Contains(we.ExperienceId) && we.Wishlist.UserId == userId)
                .Select(we => we.ExperienceId)
                .Distinct()
                .ToListAsync();
        }
    }
}
