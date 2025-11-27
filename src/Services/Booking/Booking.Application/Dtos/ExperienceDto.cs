namespace Booking.Application.Dtos
{
    public class ExperienceDto
    {
        public Guid Id { get; set; }
        public Guid HostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal AdultPrice { get; set; }
        public decimal ChildPrice { get; set; }
        public int Duration { get; set; }
        public int MaxParticipants { get; set; }
        public string CancellationPolicy { get; set; } = string.Empty;
    }
}
