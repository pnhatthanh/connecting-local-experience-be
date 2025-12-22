namespace Experience.Application.Dtos
{
    public class ExperienceSummaryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MaxParticipants { get; set; }
        public string Address { get; set; } = string.Empty;
        public CategoryDto Category { get; set; } = null!;
        public decimal AdultPrice { get; set; }
        public decimal ChildPrice { get; set; }
        public int Duration { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public string Language { get; set; } = string.Empty;
        public List<ExperienceMediaDto>? Media { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsFavorite { get; set; }
    }
}