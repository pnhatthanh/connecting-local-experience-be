using Microsoft.AspNetCore.Http;

namespace Booking.Application.Interfaces
{
    public interface IVnPayService
    {
        Task<(bool success, string paymentUrl, string transactionId)> CreatePaymentUrlAsync(
            Guid bookingId, decimal amount, string bookingCode);

        Task<(bool isValid, string message, string transactionNo)> ValidateCallbackAsync(
            IQueryCollection? queryParams);

        (bool isValid, string message) ValidateIpnCallback(IQueryCollection? queryParams);
    }
}