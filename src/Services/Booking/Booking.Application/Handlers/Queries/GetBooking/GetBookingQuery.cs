using BuildingBlocks.Application.CQRS.Query;
using Booking.Application.Dtos;

namespace Booking.Application.Handlers.Queries.GetBooking
{
    public record GetBookingQuery(Guid BookingId) : IQuery<BookingDto?>;
}
