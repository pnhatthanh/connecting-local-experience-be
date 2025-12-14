using BuildingBlocks.Application.CQRS.Query;
using Booking.Application.Dtos;

namespace Booking.Application.Handlers.Queries.GetHostBookings
{
    public record GetHostBookingsQuery : IQuery<IEnumerable<BookingDto>>
    {
        public DateOnly Date { get; init; } = DateOnly.FromDateTime(DateTime.Now);
        public TimeSpan? StartTime { get; init; }
    }
}
