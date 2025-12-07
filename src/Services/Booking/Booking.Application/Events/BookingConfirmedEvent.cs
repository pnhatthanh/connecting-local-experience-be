using BuildingBlocks.Application.EventBus.Events;

namespace Booking.Application.Events
{
    public class BookingConfirmedEvent : IntegrationEvent
    {
        public Guid BookingId { get; set; }
        public Guid ExperienceId { get; set; }
        public DateOnly Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public int TotalParticipants => Adults + Children;
        
        public BookingConfirmedEvent() { }
        
        public BookingConfirmedEvent(Guid bookingId, Guid experienceId, DateOnly date, 
            TimeSpan startTime, TimeSpan endTime, int adults, int children)
        {
            BookingId = bookingId;
            ExperienceId = experienceId;
            Date = date;
            StartTime = startTime;
            EndTime = endTime;
            Adults = adults;
            Children = children;
        }
    }
}
