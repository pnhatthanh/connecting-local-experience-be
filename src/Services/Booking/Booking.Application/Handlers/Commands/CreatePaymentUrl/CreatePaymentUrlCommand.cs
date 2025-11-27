using BuildingBlocks.Application.CQRS.Command;

namespace Booking.Application.Handlers.Commands.CreatePaymentUrl
{
    public record CreatePaymentUrlCommand(Guid BookingId, string IpAddress) : ICommand<CreatePaymentUrlResponse>;

    public record CreatePaymentUrlResponse(
        bool Success,
        string PaymentUrl,
        string Provider,
        string Message
    );
}
