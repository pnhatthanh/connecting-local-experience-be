using BuildingBlocks.Application.CQRS.Command;

namespace Booking.Application.Handlers.Commands.ToggleBookingStatus
{
    public record ToggleBookingStatusCommand(
        Guid BookingId,
        string Status
    ) : ICommand<bool>;
}
