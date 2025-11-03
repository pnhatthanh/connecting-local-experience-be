using BuildingBlocks.Domain.Models;
using User.Domain.Enums;

namespace User.Domain.Entities
{
    public class UserEntity : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateOnly? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Country { get; set; }
        public UserRole Role { get; set; } = UserRole.User;
        public UserStatus Status { get; set; } = UserStatus.Active;
        public virtual HostProfileEntity? HostProfile { get; set; }
        public virtual ICollection<UserFavoriteExperienceEntity> FavoriteExperiences { get; set; } = new List<UserFavoriteExperienceEntity>();
    }
}
