namespace UserService.Application.DTOs.Admin
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public int Name { get; set; }
        public string NameDisplay { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
