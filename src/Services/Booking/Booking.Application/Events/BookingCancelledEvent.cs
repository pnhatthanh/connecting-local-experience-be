using BuildingBlocks.Application.EventBus.Events;

namespace Booking.Application.Events
{
    public class BookingCancelledEvent : IntegrationEvent
    {
        public Guid BookingId { get; set; }
        public Guid ExperienceId { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public string CancelledBy { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public decimal RefundAmount { get; set; }
        
        public BookingCancelledEvent() { }
        
        public BookingCancelledEvent(Guid bookingId, Guid experienceId, string bookingCode, 
            DateTime startTime, DateTime endTime, int adults, int children, 
            string cancelledBy, string reason, decimal refundAmount)
        {
            BookingId = bookingId;
            ExperienceId = experienceId;
            BookingCode = bookingCode;
            StartTime = startTime;
            EndTime = endTime;
            Adults = adults;
            Children = children;
            CancelledBy = cancelledBy;
            Reason = reason;
            RefundAmount = refundAmount;
        }
    }
}
