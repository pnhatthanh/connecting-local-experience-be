namespace Booking.Application.Dtos;

public class BookingSummaryDto
{
    public Guid Id { get; set; }
    public Guid ExperienceId { get; set; }
    public string ExperienceTitle { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string BookingCode { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public int Adults { get; set; } = 1;
    public int Children { get; set; } = 0;
    public decimal TotalPrice { get; set; }  
    public string Status { get; set; } = string.Empty;
}