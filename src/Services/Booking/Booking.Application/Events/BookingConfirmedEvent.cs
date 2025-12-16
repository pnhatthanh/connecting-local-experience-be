using BuildingBlocks.Application.EventBus.Events;

namespace Booking.Application.Events
{
    public class BookingConfirmedEvent : IntegrationEvent
    {
        public Guid BookingId { get; set; }
        public Guid HostId { get; set; }
        public Guid UserId { get; set; }
        public Guid ExperienceId { get; set; }
        public DateOnly Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        
        public BookingConfirmedEvent() { }
        
        public BookingConfirmedEvent(Guid bookingId, Guid hostId, Guid experienceId, Guid userId, DateOnly date, 
            TimeSpan startTime, TimeSpan endTime, int adults, int children)
        {
            BookingId = bookingId;
            HostId = hostId;
            ExperienceId = experienceId;
            UserId = userId;
            Date = date;
            StartTime = startTime;
            EndTime = endTime;
            Adults = adults;
            Children = children;
        }
    }
}
