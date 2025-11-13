namespace Booking.Application.Dtos
{
    public class RefundDto
    {
        public Guid Id { get; set; }
        public Guid? PaymentId { get; set; }
        public Guid BookingId { get; set; }
        public decimal RefundAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}
