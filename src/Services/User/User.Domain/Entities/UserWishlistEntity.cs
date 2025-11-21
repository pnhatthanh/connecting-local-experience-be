using BuildingBlocks.Domain.Models;

namespace User.Domain.Entities
{
    public class UserWishlistEntity : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ExperienceCount { get; set; } = 0;
        public virtual UserEntity User { get; set; } = null!;
        public virtual ICollection<WishlistExperienceEntity> WishlistExperiences { get; set; } = new List<WishlistExperienceEntity>();
    }
}
