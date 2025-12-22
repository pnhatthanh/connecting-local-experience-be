using BuildingBlocks.EntityFramework;
using Booking.Domain.Entities;
using Booking.Domain.Repositories;
using Booking.Infrastructure.Data;

namespace Booking.Infrastructure.Repositories
{
    public class BookingRepository(BookingDbContext context) : BaseRepository<BookingEntity>(context), IBookingRepository
    {
    }
}
