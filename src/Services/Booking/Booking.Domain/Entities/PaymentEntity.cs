using BuildingBlocks.Domain.Models;
using Booking.Domain.Enums;

namespace Booking.Domain.Entities
{
    public class PaymentEntity : BaseEntity
    {
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "VND";
        public PaymentProvider Provider { get; set; } = PaymentProvider.VnPay;
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string? TransactionId { get; set; }   
        public string? PaymentUrl { get; set; }         
        public DateTime? PaidAt { get; set; }
        public virtual BookingEntity Booking { get; set; } = null!;
    }
}
