using BuildingBlocks.Application.CQRS.Query;
using Booking.Application.Dtos;

namespace Booking.Application.Handlers.Queries.GetHostBookings
{
    public record GetHostBookingsQuery(Guid HostId) : IQuery<IEnumerable<BookingDto>>;
}
