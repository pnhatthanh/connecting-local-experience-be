namespace User.Application.DTOs
{
    public record WishlistDto(
        Guid Id,
        string Name,
        int ExperienceCount,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );
     
}
