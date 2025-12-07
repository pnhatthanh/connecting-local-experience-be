namespace Booking.Application.Dtos
{
    public class ExperienceDto
    {
        public Guid Id { get; set; }
        public Guid HostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal AdultPrice { get; set; }
        public decimal ChildPrice { get; set; }
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public int Duration { get; set; }
        public int MaxParticipants { get; set; }
        public string CancellationPolicy { get; set; } = string.Empty;
        public List<ExperienceMediaDto>? Media { get; set; }
    }
    public class ExperienceMediaDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}
