namespace User.Application.DTOs;

public record UserSummaryDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
    public string? Country { get; init; }
    public string Email { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string Role { get; init; } = string.Empty;
    public DateOnly JoinedDate { get; init; }
    public bool IsActive { get; init; }

}