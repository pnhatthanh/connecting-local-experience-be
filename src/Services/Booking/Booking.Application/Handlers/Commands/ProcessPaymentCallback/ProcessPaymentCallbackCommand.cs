using BuildingBlocks.Application.CQRS.Command;

namespace Booking.Application.Handlers.Commands.ProcessPaymentCallback
{
    public record ProcessMomoCallbackCommand(Dictionary<string, string> QueryParams) 
        : ICommand<ProcessPaymentCallbackResponse>;

    public record ProcessPaymentCallbackResponse(
        bool Success,
        string Message,
        string? BookingCode = null
    );
}
