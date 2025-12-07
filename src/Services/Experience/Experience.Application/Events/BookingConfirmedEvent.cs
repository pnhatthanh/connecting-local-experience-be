using BuildingBlocks.Application.EventBus.Events;

namespace Experience.Application.Events
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
        public int TotalParticipants { get; set; }
        
        public BookingConfirmedEvent() { }
        public BookingConfirmedEvent(Guid bookingId, Guid experienceId, DateOnly date, 
            TimeSpan startTime, TimeSpan endTime, int adults, int children, int totalParticipants)
        {
            BookingId = bookingId;
            ExperienceId = experienceId;
            Date = date;
            StartTime = startTime;
            EndTime = endTime;
            Adults = adults;
            Children = children;
            TotalParticipants = totalParticipants;
        }

    }
}
