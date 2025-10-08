namespace IAM.Application.DTOs
{
    public class RegisterResponse
    {
        public Guid AccountId { get; set; }
        public string? FullName { get; set; } 
        public string? Email { get; set; } 
    }
}