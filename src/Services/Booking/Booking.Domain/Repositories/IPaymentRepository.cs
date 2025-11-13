using BuildingBlocks.Domain.Interfaces;
using Booking.Domain.Entities;

namespace Booking.Domain.Repositories
{
    public interface IPaymentRepository : IRepository<PaymentEntity>
    {
        Task<PaymentEntity?> GetByBookingIdAsync(Guid bookingId);
        Task<PaymentEntity?> GetByVnpTxnRefAsync(string vnpTxnRef);
    }
}
