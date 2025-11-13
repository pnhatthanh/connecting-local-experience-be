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
        public PaymentMethod? Method { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        
        // VNPay specific fields
        public string? VnpTxnRef { get; set; }
        public string? VnpResponseCode { get; set; }
        public string? VnpBankCode { get; set; }
        public string? VnpSecureHash { get; set; }
        public DateTime? PaidAt { get; set; }
        
        // Navigation properties
        public virtual BookingEntity Booking { get; set; } = null!;
        public virtual ICollection<RefundEntity> Refunds { get; set; } = [];
    }
}
