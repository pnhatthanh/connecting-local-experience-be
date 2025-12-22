namespace Booking.Application.Dtos;

public class BookingStatisticsDto
{
    public int TotalBookings { get; set; }
    public int CurrentMonthBookings { get; set; }
    public decimal CurrentMonthBookingsGrowthRate { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal CurrentMonthRevenue { get; set; }
    public decimal CurrentMonthRevenueGrowthRate { get; set; }
    public int ConfirmedBookings { get; set; }
    public int CancelledBookings { get; set; }
    public decimal ConversionRate { get; set; }
    public decimal AverageBookingValue { get; set; }
    public List<MonthlyBookingTrendDto> MonthlyTrends { get; set; } = new();
    public List<MonthlyRevenueTrendDto> MonthlyRevenueTrends { get; set; } = new();
}

public class MonthlyBookingTrendDto
{
    public int Month { get; set; }
    public string MonthLabel { get; set; } = string.Empty; // T1, T2, T3...
    public int NewBookings { get; set; }
    public int ConfirmedBookings { get; set; }
}
public class MonthlyRevenueTrendDto
{
    public int Month { get; set; }
    public string MonthLabel { get; set; } = string.Empty; // T1, T2, T3...
    public decimal Revenue { get; set; }
}
