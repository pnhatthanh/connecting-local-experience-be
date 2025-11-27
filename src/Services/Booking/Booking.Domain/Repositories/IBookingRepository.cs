using BuildingBlocks.Domain.Interfaces;
using Booking.Domain.Entities;

namespace Booking.Domain.Repositories
{
    public interface IBookingRepository : IBaseRepository<BookingEntity>
    {
    }
}
