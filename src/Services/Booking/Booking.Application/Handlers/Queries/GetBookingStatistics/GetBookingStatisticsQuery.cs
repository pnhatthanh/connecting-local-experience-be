using Booking.Application.Dtos;
using MediatR;

namespace Booking.Application.Handlers.Queries.GetBookingStatistics;

public record GetBookingStatisticsQuery(
    int Year = 2025
) : IRequest<BookingStatisticsDto>;
