using BuildingBlocks.Application.CQRS.Command;

namespace Booking.Application.Handlers.Commands.CancelBooking
{
    public record CancelBookingCommand(
        Guid BookingId,
        string Reason,
        bool IsCancelledByHost = false
    ) : ICommand<bool>;
}
