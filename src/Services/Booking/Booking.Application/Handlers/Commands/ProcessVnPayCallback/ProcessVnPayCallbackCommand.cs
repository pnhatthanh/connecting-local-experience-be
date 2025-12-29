using BuildingBlocks.Application.CQRS.Command;
using Microsoft.AspNetCore.Http;

namespace Booking.Application.Handlers.Commands.ProcessVnPayCallback
{
    public record ProcessVnPayCallbackCommand(
        IQueryCollection? QueryParams
    ) : ICommand<ProcessVnPayCallbackResponse>;

    public record ProcessVnPayCallbackResponse(
        bool Success,
        string Message,
        string ClientType,
        string? BookingCode = null
    );
}
