using Booking.Application.Dtos;
using BuildingBlocks.Application.CQRS.Query;

namespace Booking.Application.Handlers.Queries.GetExperienceBookings
{
    public record GetExperienceBookingsQuery(
        Guid ExperienceId,
        DateOnly? Date = null,
        TimeSpan? StartTime = null
    ) : IQuery<List<BookingDto>>;
}
