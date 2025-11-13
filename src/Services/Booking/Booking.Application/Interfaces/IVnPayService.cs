namespace Booking.Application.Interfaces
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(Guid bookingId, decimal amount, string bookingCode, string ipAddress);
        Task<(bool isValid, string responseCode)> ValidateCallbackAsync(Dictionary<string, string> queryParams);
        Task<(bool success, string responseCode)> ProcessRefundAsync(Guid paymentId, decimal refundAmount, string reason);
    }
}
