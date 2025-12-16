namespace Booking.Application.Dtos;

public class TopHostStatisticsDto
{
    public Guid HostId { get; set; }
    public string HostName { get; set; } = string.Empty;
    public int ExperienceCount { get; set; }
    public int TotalBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageRating { get; set; }
}

public class HostProfileDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int TotalExperiences { get; set; }
    public decimal RatingAvg { get; set; }
}
