using BuildingBlocks.Application.CQRS.Command;
using Booking.Application.Dtos;

namespace Booking.Application.Handlers.Commands.CreateBooking
{
    public record CreateBookingCommand(
        Guid ExperienceId,
        DateTime StartTime,
        DateTime EndTime,
        int Adults,
        int Children,
        string ContactName,
        string ContactEmail,
        string ContactPhone,
        string? Notes
    ) : ICommand<BookingDto>;
}
