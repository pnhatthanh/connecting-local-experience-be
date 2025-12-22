namespace User.Application.DTOs
{
    public record WishlistDetailDto(
        Guid Id,
        string Name,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        List<ExperienceSummaryDto> Experiences
    );
}