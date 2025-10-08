using BuildingBlocks.Domain.Models;

namespace IAM.Domain.Entities
{
    public class RefreshTokenEntity : BaseEntity
    {
        public Guid AccountId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; } = false;
        public DateTime? RevokedAt { get; set; }
        public virtual AccountEntity Account { get; set; } = null!;

        public bool IsActive => !IsRevoked && DateTime.UtcNow <= ExpiryDate;
    }
}