using Booking.Application.Dtos;
using MediatR;

namespace Booking.Application.Handlers.Queries.GetTopHosts;

public record GetTopHostsQuery(
    int Year = 2025, 
    int Limit = 10
) : IRequest<List<TopHostStatisticsDto>>;
