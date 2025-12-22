using BuildingBlocks.Application.EventBus.Events;

namespace Booking.Application.Events
{
    public class BookingConfirmedEvent : IntegrationEvent
    {
        public Guid BookingId { get; set; }
        public Guid HostId { get; set; }
        public Guid UserId { get; set; }
    }
}
