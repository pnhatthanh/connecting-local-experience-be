using BuildingBlocks.Domain.Models;

namespace User.Domain.Entities
{
    public class WishlistExperienceEntity : BaseEntity
    {
        public Guid WishlistId { get; set; }
        public Guid ExperienceId { get; set; }
        public virtual UserWishlistEntity Wishlist { get; set; } = null!;
    }
}
