using BuildingBlocks.Domain.Models;

namespace User.Domain.Entities
{
    public class UserFavoriteExperienceEntity : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid ExperienceId { get; set; }
        public virtual UserEntity User { get; set; } = null!;
    }
}
