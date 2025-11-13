namespace Booking.Application.Dtos
{
    public class BookingCancellationDto
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public string CancelledBy { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public decimal CancellationFee { get; set; }
        public decimal RefundAmount { get; set; }
        public DateTime CancelledAt { get; set; }
    }
}
