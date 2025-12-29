using BuildingBlocks.Application.CQRS.Command;

namespace Booking.Application.Handlers.Commands.CreateBooking
{
    public record CreateBookingCommand(
        Guid ExperienceId,
        DateOnly Date,
        TimeSpan StartTime,
        TimeSpan EndTime,
        int Adults,
        int Children,
        string FirstName,
        string LastName,
        string ContactEmail,
        string ContactPhone,
        string? Notes,
        string ClientType = "Web",
        string? IpAddress = null
    ) : ICommand<CreateBookingResponse>;

    public record CreateBookingResponse(
        bool Success,
        string? PaymentUrl,
        string Message
    );
}
