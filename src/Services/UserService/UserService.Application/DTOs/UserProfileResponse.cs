namespace UserService.Application.DTOs
{
    public class UserProfileResponse
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Nationality { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
