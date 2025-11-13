using BuildingBlocks.Application.CQRS.Query;
using Booking.Application.Dtos;

namespace Booking.Application.Handlers.Queries.GetUserBookings
{
    public record GetUserBookingsQuery(Guid UserId) : IQuery<IEnumerable<BookingDto>>;
}
