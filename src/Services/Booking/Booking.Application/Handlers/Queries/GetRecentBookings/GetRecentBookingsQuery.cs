using Booking.Application.Dtos;
using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Application.Dtos;

namespace Booking.Application.Handlers.Queries.GetRecentBookings
{
    public record GetRecentBookingsQuery(
        int PageNumber = 1,
        int PageSize = 10
    ) : IQuery<PaginationResult<BookingDto>>;
}