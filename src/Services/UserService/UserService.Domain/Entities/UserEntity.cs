using BuildingBlocks.Domain.Models;

namespace UserService.Domain.Entities
{
    public class ProfileEntity : BaseEntity
    {
        public Guid AccountId { get; set; } // Reference to IAM Account
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Nationality { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
