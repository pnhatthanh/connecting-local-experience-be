using BuildingBlocks.Domain.Interfaces;
using User.Domain.Entities;

namespace User.Domain.Repositories
{
    public interface IWishlistExperienceRepository : IBaseRepository<WishlistExperienceEntity>
    {
        Task<List<Guid>> GetFavoriteExperienceIdsByUserIdAsync(Guid userId, List<Guid> experienceIds);
    }
}
