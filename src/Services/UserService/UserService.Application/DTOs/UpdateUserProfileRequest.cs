namespace UserService.Application.DTOs
{
    public class UpdateUserProfileRequest
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Nationality { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
