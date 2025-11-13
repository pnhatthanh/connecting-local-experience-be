using BuildingBlocks.EntityFramework;
using Booking.Domain.Entities;
using Booking.Domain.Repositories;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Repositories
{
    public class BookingRepository : BaseRepository<BookingEntity>, IBookingRepository
    {
        private readonly BookingDbContext _context;

        public BookingRepository(BookingDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<BookingEntity?> GetByBookingCodeAsync(string bookingCode)
        {
            return await _context.Bookings
                .Include(b => b.Payment)
                .Include(b => b.Cancellation)
                .FirstOrDefaultAsync(b => b.BookingCode == bookingCode);
        }

        public async Task<IEnumerable<BookingEntity>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Bookings
                .Include(b => b.Payment)
                .Include(b => b.Cancellation)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<BookingEntity>> GetByHostIdAsync(Guid hostId)
        {
            return await _context.Bookings
                .Include(b => b.Payment)
                .Include(b => b.Cancellation)
                .Where(b => b.HostId == hostId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<BookingEntity>> GetByExperienceIdAsync(Guid experienceId)
        {
            return await _context.Bookings
                .Include(b => b.Payment)
                .Include(b => b.Cancellation)
                .Where(b => b.ExperienceId == experienceId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<string> GenerateBookingCodeAsync()
        {
            var today = DateTime.UtcNow;
            var datePrefix = today.ToString("yyyyMMdd");
            
            // Get the latest booking code for today
            var lastBooking = await _context.Bookings
                .Where(b => b.BookingCode.StartsWith("BK" + datePrefix))
                .OrderByDescending(b => b.BookingCode)
                .FirstOrDefaultAsync();

            int sequenceNumber = 1;
            if (lastBooking != null)
            {
                var lastSequence = lastBooking.BookingCode.Substring(10); // After "BK20251113"
                if (int.TryParse(lastSequence, out int lastNumber))
                {
                    sequenceNumber = lastNumber + 1;
                }
            }

            return $"BK{datePrefix}{sequenceNumber:D3}";
        }

        public override async Task<BookingEntity?> GetByIdAsync(Guid id)
        {
            return await _context.Bookings
                .Include(b => b.Payment)
                .Include(b => b.Cancellation)
                .FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}
