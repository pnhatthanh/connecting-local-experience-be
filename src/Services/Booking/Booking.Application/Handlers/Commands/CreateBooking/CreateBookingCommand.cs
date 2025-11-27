using BuildingBlocks.Application.CQRS.Command;
using Booking.Application.Dtos;
using Booking.Domain.Enums;

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
        PaymentProvider PaymentProvider = PaymentProvider.Momo  
    ) : ICommand<BookingDto>;
}
