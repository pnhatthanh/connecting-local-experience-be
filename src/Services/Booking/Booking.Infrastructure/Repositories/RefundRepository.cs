using BuildingBlocks.EntityFramework;
using Booking.Domain.Entities;
using Booking.Domain.Repositories;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Repositories
{
    public class RefundRepository : BaseRepository<RefundEntity>, IRefundRepository
    {
        private readonly BookingDbContext _context;

        public RefundRepository(BookingDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RefundEntity>> GetByBookingIdAsync(Guid bookingId)
        {
            return await _context.Refunds
                .Where(r => r.BookingId == bookingId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<RefundEntity?> GetByVnpRefundRefAsync(string vnpRefundRef)
        {
            return await _context.Refunds
                .FirstOrDefaultAsync(r => r.VnpRefundRef == vnpRefundRef);
        }
    }
}
