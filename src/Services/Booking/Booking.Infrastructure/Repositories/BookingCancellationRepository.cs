using BuildingBlocks.EntityFramework;
using Booking.Domain.Entities;
using Booking.Domain.Repositories;
using Booking.Infrastructure.Data;

namespace Booking.Infrastructure.Repositories
{
    public class BookingCancellationRepository(BookingDbContext context) : BaseRepository<BookingCancellationEntity>(context), IBookingCancellationRepository
    {
    }
}
