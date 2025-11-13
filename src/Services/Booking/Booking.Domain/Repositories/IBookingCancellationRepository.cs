using BuildingBlocks.Domain.Interfaces;
using Booking.Domain.Entities;

namespace Booking.Domain.Repositories
{
    public interface IBookingCancellationRepository : IRepository<BookingCancellationEntity>
    {
        Task<BookingCancellationEntity?> GetByBookingIdAsync(Guid bookingId);
    }
}
