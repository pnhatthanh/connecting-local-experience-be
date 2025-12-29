using BuildingBlocks.Domain.Models;
using Booking.Domain.Enums;

namespace Booking.Domain.Entities
{
    public class BookingEntity : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid HostId { get; set; }
        public Guid ExperienceId { get; set; }
        public string ExperienceTitle { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string BookingCode { get; set; } = string.Empty;
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public DateOnly Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Location { get; set; } = string.Empty;
        public int Adults { get; set; } = 1;
        public int Children { get; set; } = 0;
        public decimal TotalPrice { get; set; }                    
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public ClientType ClientType { get; set; } = ClientType.Web;
        public virtual PaymentEntity? Payment { get; set; }
        public virtual BookingCancellationEntity? Cancellation { get; set; }
        public virtual RefundEntity? Refunds { get; set; }
    }
}
