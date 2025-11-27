namespace Booking.Domain.Enums
{
    public enum PayoutStatus
    {
        Pending,        // Waiting for scheduled date
        Processing,     // Currently processing bank transfer
        Completed,      // Successfully transferred to host
        Failed,         // Transfer failed (wrong bank info, etc.)
        Cancelled       // Booking was cancelled/refunded
    }
}
