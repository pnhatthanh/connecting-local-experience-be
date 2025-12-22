using User.Domain.Enums;

namespace User.Application.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; } 
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateOnly? DateOfBirth { get; set; }
        public string? Gender { get; set; } 
        public string? AvatarUrl { get; set; }
        public string? Country { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public HostProfileDto? HostProfile { get; set; }
    }
}
