namespace Booking.Application.Handlers.Queries.GetHostStatistics
{
    public class GetHostStatisticsResponse
    {
        public decimal TotalRevenue { get; set; }
        public int TotalBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public decimal AverageBookingValue { get; set; }
        public decimal GrowthRate { get; set; }
        public decimal RevenueGrowthRate { get; set; }
        public decimal BookingSuccessRate { get; set; }
        public List<MonthlyHostStats> MonthlyTrends { get; set; } = new();
    }

    public class MonthlyHostStats
    {
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int BookingCount { get; set; }
        public int CompletedCount { get; set; }
        public int CancelledCount { get; set; }
    }
}
