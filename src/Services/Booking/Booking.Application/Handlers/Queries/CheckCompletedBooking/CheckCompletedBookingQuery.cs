using BuildingBlocks.Application.CQRS.Query;

namespace Booking.Application.Handlers.Queries.CheckCompletedBooking
{
    public record CheckCompletedBookingQuery(
        Guid UserId, 
        Guid ExperienceId) : IQuery<bool>;
}
