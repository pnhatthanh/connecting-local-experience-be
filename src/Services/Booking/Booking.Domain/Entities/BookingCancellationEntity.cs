using BuildingBlocks.Domain.Models;
using Booking.Domain.Enums;

namespace Booking.Domain.Entities
{
    public class BookingCancellationEntity : BaseEntity
    {
        public Guid BookingId { get; set; }
        public CancelledBy CancelledBy { get; set; }
        public string Reason { get; set; } = string.Empty;
        public decimal CancellationFee { get; set; } = 0;
        public decimal RefundAmount { get; set; } = 0;
        public DateTime CancelledAt { get; set; }
        public virtual BookingEntity Booking { get; set; } = null!;
    }
}
