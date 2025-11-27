namespace Booking.Application.Interfaces
{
    public interface IMomoService
    {
        /// <summary>
        /// Create Momo payment URL for user to pay
        /// </summary>
        Task<(bool success, string paymentUrl, string message)> CreatePaymentUrlAsync(
            Guid bookingId, 
            decimal amount, 
            string bookingCode, 
            string ipAddress);

        /// <summary>
        /// Validate Momo callback signature and process payment
        /// </summary>
        Task<(bool isValid, string message, string transactionId)> ValidateCallbackAsync(
            Dictionary<string, string> queryParams);

        /// <summary>
        /// Process refund through Momo
        /// </summary>
        Task<(bool success, string message, string refundId)> ProcessRefundAsync(
            Guid paymentId, 
            decimal refundAmount, 
            string reason);
    }
}
