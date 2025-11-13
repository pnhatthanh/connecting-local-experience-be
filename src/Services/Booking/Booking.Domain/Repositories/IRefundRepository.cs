using BuildingBlocks.Domain.Interfaces;
using Booking.Domain.Entities;

namespace Booking.Domain.Repositories
{
    public interface IRefundRepository : IRepository<RefundEntity>
    {
        Task<IEnumerable<RefundEntity>> GetByBookingIdAsync(Guid bookingId);
        Task<RefundEntity?> GetByVnpRefundRefAsync(string vnpRefundRef);
    }
}
