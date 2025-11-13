using BuildingBlocks.Application.EventBus.Events;

namespace Booking.Application.Events
{
    public class BookingCompletedEvent : IntegrationEvent
    {
        public Guid BookingId { get; set; }
        public Guid ExperienceId { get; set; }
        public Guid UserId { get; set; }
        public Guid HostId { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        
        public BookingCompletedEvent() { }
        
        public BookingCompletedEvent(Guid bookingId, Guid experienceId, Guid userId, Guid hostId, 
            string bookingCode, DateTime startTime, DateTime endTime)
        {
            BookingId = bookingId;
            ExperienceId = experienceId;
            UserId = userId;
            HostId = hostId;
            BookingCode = bookingCode;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
