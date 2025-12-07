namespace Experience.Application.Dtos
{
    public record ReviewDto
    {
        public Guid Id { get; init; }
        public Guid ExperienceId { get; init; }
        public Guid UserId { get; init; }
        public string FullName { get; init; } = string.Empty;
        public string? UserAvatar { get; init; }
        public int Rating { get; init; }
        public string Description { get; init; } = string.Empty;
        public bool IsHidden { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
