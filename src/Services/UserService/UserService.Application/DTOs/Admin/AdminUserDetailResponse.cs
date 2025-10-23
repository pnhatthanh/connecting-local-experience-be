namespace UserService.Application.DTOs.Admin
{
    public class AdminUserDetailResponse
    {
        public Guid AccountId { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public Guid? RoleId { get; set; }
        public string? RoleName { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Nationality { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
