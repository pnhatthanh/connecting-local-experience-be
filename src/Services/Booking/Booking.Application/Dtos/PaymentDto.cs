namespace Booking.Application.Dtos
{
    public class PaymentDto
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string? Method { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? PaymentUrl { get; set; }
        public string? TransactionId { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
