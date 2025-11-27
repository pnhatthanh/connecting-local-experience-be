using BuildingBlocks.Domain.Models;
using Booking.Domain.Enums;

namespace Booking.Domain.Entities
{
    public class PaymentEntity : BaseEntity
    {
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "VND";
        public PaymentProvider Provider { get; set; } = PaymentProvider.Momo;
        public PaymentMethod? Method { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        
        // Generic transaction reference
        public string? TransactionId { get; set; }           // Gateway transaction ID
        public string? PaymentUrl { get; set; }              // Payment URL for redirect
        
        public DateTime? PaidAt { get; set; }
        public virtual BookingEntity Booking { get; set; } = null!;
        public virtual ICollection<RefundEntity> Refunds { get; set; } = [];
    }
}
