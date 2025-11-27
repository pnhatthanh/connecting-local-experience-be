using BuildingBlocks.Domain.Models;
using Booking.Domain.Enums;

namespace Booking.Domain.Entities
{
    /// <summary>
    /// Entity to track payouts to hosts after experience completion.
    /// Platform aggregates payments and pays hosts periodically.
    /// </summary>
    public class HostPayoutEntity : BaseEntity
    {
        public Guid BookingId { get; set; }
        public Guid HostId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "VND";
        public PayoutStatus Status { get; set; } = PayoutStatus.Pending;
        
        // Scheduling
        public DateTime ScheduledDate { get; set; }          // When payout should be processed
        public DateTime? ProcessedDate { get; set; }         // When payout was actually processed
        
        // Bank information (from Host profile in User Service)
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankAccountName { get; set; }
        
        // Transaction tracking
        public string? TransactionReference { get; set; }    // Bank transfer reference
        public string? FailureReason { get; set; }           // If payout failed
        
        // Navigation properties
        public virtual BookingEntity Booking { get; set; } = null!;
    }
}
