namespace User.Application.DTOs
{
    public class UserFavoriteExperienceDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ExperienceId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
