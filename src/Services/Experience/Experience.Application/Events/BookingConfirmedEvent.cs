using BuildingBlocks.Application.EventBus.Events;

namespace Experience.Application.Events
{
    public class BookingConfirmedEvent : IntegrationEvent
    {
        public Guid BookingId { get; set; }
        public Guid ExperienceId { get; set; }
        public Guid UserId { get; set; }
        public DateOnly Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
    }
}
