namespace User.Application.DTOs
{
    public class HostSummaryDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Country { get; set; }
        public string? AvatarUrl { get; set; }
        public string Location { get; set; } = null!;
        public string? Work { get; set; } = null!;
        public string? Education { get; set; } = null!;
        public bool IsVerified { get; set; }
        public string VerifyStatus { get; set; } = null!;

    }
}