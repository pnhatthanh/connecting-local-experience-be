using BuildingBlocks.Application.EventBus.Events;

namespace Booking.Application.Events
{
    public class BookingCreatedEvent : IntegrationEvent
    {
        public Guid BookingId { get; set; }
        public Guid ExperienceId { get; set; }
        public Guid UserId { get; set; }
        public Guid HostId { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public decimal TotalPrice { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        
        public BookingCreatedEvent() { }
        
        public BookingCreatedEvent(Guid bookingId, Guid experienceId, Guid userId, Guid hostId, 
            string bookingCode, DateTime startTime, DateTime endTime, int adults, int children, 
            decimal totalPrice, string contactName, string contactEmail)
        {
            BookingId = bookingId;
            ExperienceId = experienceId;
            UserId = userId;
            HostId = hostId;
            BookingCode = bookingCode;
            StartTime = startTime;
            EndTime = endTime;
            Adults = adults;
            Children = children;
            TotalPrice = totalPrice;
            ContactName = contactName;
            ContactEmail = contactEmail;
        }
    }
}
