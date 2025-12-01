using BuildingBlocks.Domain.Models;
using Booking.Domain.Enums;

namespace Booking.Domain.Entities
{
    public class BookingEntity : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid HostId { get; set; }
        public Guid ExperienceId { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public DateOnly Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Adults { get; set; } = 1;
        public int Children { get; set; } = 0;
        public decimal TotalPrice { get; set; }             
        public decimal PlatformFee { get; set; }             
        public decimal HostAmount { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public virtual PaymentEntity? Payment { get; set; }
        public virtual BookingCancellationEntity? Cancellation { get; set; }
        public virtual ICollection<RefundEntity> Refunds { get; set; } = [];
    }
}
