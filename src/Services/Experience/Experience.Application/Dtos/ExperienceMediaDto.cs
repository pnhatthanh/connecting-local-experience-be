namespace Experience.Application.Dtos
{
    public class ExperienceMediaDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}
