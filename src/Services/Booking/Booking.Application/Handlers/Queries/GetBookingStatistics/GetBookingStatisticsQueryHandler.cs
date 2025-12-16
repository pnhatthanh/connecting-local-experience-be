using Booking.Application.Dtos;
using Booking.Domain.Repositories;
using Booking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Handlers.Queries.GetBookingStatistics;

public class GetBookingStatisticsQueryHandler : IRequestHandler<GetBookingStatisticsQuery, BookingStatisticsDto>
{
    private readonly IBookingRepository _bookingRepository;

    public GetBookingStatisticsQueryHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<BookingStatisticsDto> Handle(GetBookingStatisticsQuery request, CancellationToken cancellationToken)
    {
        var year = request.Year;
        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;

        var allBookingsQuery = _bookingRepository.GetQueryable()
            .Where(b => b.CreatedAt.Year == year);

        var allBookings = await allBookingsQuery.ToListAsync(cancellationToken);
        var totalBookings = allBookings.Count;
        var currentMonthBookings = allBookings.Count(b => 
            b.CreatedAt.Year == currentYear && b.CreatedAt.Month == currentMonth);

        var lastMonth = currentMonth == 1 ? 12 : currentMonth - 1;
        var lastMonthYear = currentMonth == 1 ? currentYear - 1 : currentYear;
        var lastMonthBookings = allBookings.Count(b => 
            b.CreatedAt.Year == lastMonthYear && b.CreatedAt.Month == lastMonth);

        var growthRate = lastMonthBookings > 0 
            ? (decimal)(currentMonthBookings - lastMonthBookings) / lastMonthBookings * 100 
            : 0;

        var confirmedBookings = allBookings.Count(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed);
        var cancelledBookings = allBookings.Count(b => b.Status == BookingStatus.Cancelled);

        var conversionRate = totalBookings > 0 
            ? (decimal)confirmedBookings / totalBookings * 100 
            : 0;
        var monthlyTrends = Enumerable.Range(1, 12)
            .Select(month =>
            {
                var monthBookings = allBookings.Where(b => b.CreatedAt.Month == month).ToList();
                return new MonthlyBookingTrendDto
                {
                    Month = month,
                    MonthLabel = $"T{month}",
                    NewBookings = monthBookings.Count,
                    ConfirmedBookings = monthBookings.Count(b => 
                        b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed)
                };
            })
            .ToList();

        var totalRevenue = allBookings.Sum(b => b.TotalPrice);
        var currentMonthRevenue = allBookings
            .Where(b => b.CreatedAt.Year == currentYear && b.CreatedAt.Month == currentMonth)
            .Sum(b => b.TotalPrice);

        var lastMonthRevenue = allBookings
            .Where(b => b.CreatedAt.Year == lastMonthYear && b.CreatedAt.Month == lastMonth)
            .Sum(b => b.TotalPrice);

        var revenueGrowthRate = lastMonthRevenue > 0
            ? currentMonthRevenue - lastMonthRevenue / lastMonthRevenue * 100
            : 0;

        var averageBookingValue = totalBookings > 0
            ? totalRevenue / totalBookings
            : 0;

        var monthlyRevenueTrends = Enumerable.Range(1, 12)
            .Select(month =>
            {
                var monthRevenue = allBookings
                    .Where(b => b.CreatedAt.Month == month)
                    .Sum(b => b.TotalPrice);
                return new MonthlyRevenueTrendDto
                {
                    Month = month,
                    MonthLabel = $"T{month}",
                    Revenue = monthRevenue
                };
            })
            .ToList();

        return new BookingStatisticsDto
        {
            TotalBookings = totalBookings,
            CurrentMonthBookings = currentMonthBookings,
            CurrentMonthBookingsGrowthRate = Math.Round(growthRate, 1),
            ConfirmedBookings = confirmedBookings,
            CancelledBookings = cancelledBookings,
            ConversionRate = Math.Round(conversionRate, 1),
            MonthlyTrends = monthlyTrends,
            TotalRevenue = totalRevenue,
            CurrentMonthRevenue = currentMonthRevenue,
            CurrentMonthRevenueGrowthRate = Math.Round(revenueGrowthRate, 1),
            AverageBookingValue = Math.Round(averageBookingValue, 2),
            MonthlyRevenueTrends = monthlyRevenueTrends
        };
    }
}
