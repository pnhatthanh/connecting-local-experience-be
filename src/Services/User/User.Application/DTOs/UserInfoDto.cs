namespace User.Application.DTOs
{
    public record UserInfoDto(
        Guid Id, 
        string FullName, 
        string? Avatar
    );
}
