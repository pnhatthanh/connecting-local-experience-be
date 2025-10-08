using BuildingBlocks.Domain.Models;

namespace IAM.Domain.Entities
{
    public class AccountEntity : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsEmailConfirmed { get; set; } = false;
        public string? EmailConfirmationToken { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid RoleId { get; set; }
        public virtual RoleEntity Role { get; set; } = null!;
        public virtual ICollection<RefreshTokenEntity> RefreshTokens { get; set; } = new List<RefreshTokenEntity>();
    }
}