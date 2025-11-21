using Booking.Domain.Entities;
using BuildingBlocks.Domain.Interfaces;

namespace Booking.Domain.Repositories
{
    public interface IBookingCancellationRepository : IBaseRepository<BookingCancellationEntity>
    {
        Task<BookingCancellationEntity?> GetByBookingIdAsync(Guid bookingId);
    }
}
