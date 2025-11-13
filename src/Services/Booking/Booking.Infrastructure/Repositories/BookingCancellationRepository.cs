using BuildingBlocks.EntityFramework;
using Booking.Domain.Entities;
using Booking.Domain.Repositories;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Repositories
{
    public class BookingCancellationRepository : BaseRepository<BookingCancellationEntity>, IBookingCancellationRepository
    {
        private readonly BookingDbContext _context;

        public BookingCancellationRepository(BookingDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<BookingCancellationEntity?> GetByBookingIdAsync(Guid bookingId)
        {
            return await _context.BookingCancellations
                .FirstOrDefaultAsync(c => c.BookingId == bookingId);
        }
    }
}
