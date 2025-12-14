namespace Booking.Application.Dtos
{
    public class BookingDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid HostId { get; set; }
        public Guid ExperienceId { get; set; }
        public string ExperienceTitle { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string BookingCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public decimal TotalPrice { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public PaymentDto? Payment { get; set; }
        public BookingCancellationDto? Cancellation { get; set; }
    }
}
