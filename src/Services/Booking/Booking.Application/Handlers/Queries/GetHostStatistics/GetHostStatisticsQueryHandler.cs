using BuildingBlocks.Application.CQRS.Query;
using Booking.Application.Interfaces;
using Booking.Domain.Enums;
using Booking.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Globalization;
using BuildingBlocks.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Handlers.Queries.GetHostStatistics
{
    public class GetHostStatisticsQueryHandler : IQueryHandler<GetHostStatisticsQuery, GetHostStatisticsResponse>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetHostStatisticsQueryHandler> _logger;

        public GetHostStatisticsQueryHandler(
            IBookingRepository bookingRepository,
            ICurrentUserService currentUserService,
            ILogger<GetHostStatisticsQueryHandler> logger)
        {
            _bookingRepository = bookingRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<GetHostStatisticsResponse> Handle(GetHostStatisticsQuery request, CancellationToken cancellationToken)
        {
            var hostId = _currentUserService.UserId;
            
            _logger.LogInformation("Getting statistics for Host {HostId}, Year {Year}", hostId, request.Year);

            var bookingsQuery =  _bookingRepository.GetQueryable()
                .Where(b => b.HostId == hostId 
                    && b.CreatedAt.Year == request.Year);
            var hostBookings = await bookingsQuery.Where(b => b.Status != BookingStatus.Pending).ToListAsync(cancellationToken);

            var totalRevenue = hostBookings
                .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed)
                .Sum(b => b.TotalPrice);

            var completedBookings = hostBookings.Count(b => b.Status == BookingStatus.Completed);
            var cancelledBookings = hostBookings.Count(b => b.Status == BookingStatus.Cancelled);

            var monthlyTrends = Enumerable.Range(1, 12)
                .Select(month =>
                {
                    var monthBookings = hostBookings.Where(b => b.CreatedAt.Month == month).ToList();
                    var monthRevenue = monthBookings
                        .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed)
                        .Sum(b => b.TotalPrice);

                    return new MonthlyHostStats
                    {
                        Month = month,
                        MonthName = $"T{month}",
                        Revenue = monthRevenue,
                        BookingCount = monthBookings.Count,
                        CompletedCount = monthBookings.Count(b => b.Status == BookingStatus.Completed),
                        CancelledCount = monthBookings.Count(b => b.Status == BookingStatus.Cancelled)
                    };
                })
                .ToList();

            var firstHalfRevenue = monthlyTrends.Take(6).Sum(m => m.Revenue);
            var secondHalfRevenue = monthlyTrends.Skip(6).Sum(m => m.Revenue);
            var growthRate = firstHalfRevenue > 0 
                ? (secondHalfRevenue - firstHalfRevenue) / firstHalfRevenue * 100 
                : 0;

            var averageBookingValue = hostBookings.Count > 0 
                ? totalRevenue / hostBookings.Count 
                : 0;

            // Revenue growth rate (current month vs previous month)
            var currentMonth = DateTime.UtcNow.Month;
            var currentMonthRevenue = monthlyTrends.FirstOrDefault(m => m.Month == currentMonth)?.Revenue ?? 0;
            var previousMonthRevenue = currentMonth > 1 
                ? monthlyTrends.FirstOrDefault(m => m.Month == currentMonth - 1)?.Revenue ?? 0
                : 0;
            var revenueGrowthRate = previousMonthRevenue > 0
                ? (currentMonthRevenue - previousMonthRevenue) / previousMonthRevenue * 100
                : 0;

            var bookingSuccessRate = hostBookings.Count > 0
                ? (decimal)completedBookings / hostBookings.Count * 100
                : 0;


            var now = DateTime.UtcNow;
            var currentWeekStart = now.AddDays(-(int)now.DayOfWeek);
            var previousWeekStart = currentWeekStart.AddDays(-7);
            

            return new GetHostStatisticsResponse
            {
                TotalRevenue = totalRevenue,
                TotalBookings = hostBookings.Count,
                CompletedBookings = completedBookings,
                CancelledBookings = cancelledBookings,
                AverageBookingValue = averageBookingValue,
                GrowthRate = Math.Round(growthRate, 2),
                RevenueGrowthRate = Math.Round(revenueGrowthRate, 2),
                BookingSuccessRate = Math.Round(bookingSuccessRate, 2),
                MonthlyTrends = monthlyTrends
            };
        }
    }
}
