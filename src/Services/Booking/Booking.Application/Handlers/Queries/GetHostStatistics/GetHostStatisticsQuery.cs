using BuildingBlocks.Application.CQRS.Query;

namespace Booking.Application.Handlers.Queries.GetHostStatistics
{
    public record GetHostStatisticsQuery(int Year = 2025) : IQuery<GetHostStatisticsResponse>;
}
