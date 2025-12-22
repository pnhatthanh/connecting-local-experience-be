using BuildingBlocks.Domain.Models;
using Booking.Domain.Enums;

namespace Booking.Domain.Entities
{
    public class RefundEntity : BaseEntity
    {
        public Guid? PaymentId { get; set; }
        public Guid BookingId { get; set; }
        public decimal RefundAmount { get; set; }
        public string Currency { get; set; } = "VND";
        public string Reason { get; set; } = string.Empty;
        public string? VnpRefundRef { get; set; }
        public string? VnpResponseCode { get; set; }
        public RefundStatus Status { get; set; } = RefundStatus.Requested;
        public DateTime RequestedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public virtual BookingEntity Booking { get; set; } = null!;
    }
}
