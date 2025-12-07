namespace Experience.Application.Dtos;

 public record UserInfoDto(
    Guid Id, 
    string FullName, 
    string? Avatar
);