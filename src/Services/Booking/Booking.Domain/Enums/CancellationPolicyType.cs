namespace Booking.Domain.Enums
{
    public enum CancellationPolicyType
    {
        AlwaysFreeCancellation,      // 100% refund anytime
        FreeCancellation24Hours,     // 100% refund if >24h before, 50% if <24h
        NonRefundable                // No refund
    }
}
