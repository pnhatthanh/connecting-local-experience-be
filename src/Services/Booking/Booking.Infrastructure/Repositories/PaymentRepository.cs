using BuildingBlocks.EntityFramework;
using Booking.Domain.Entities;
using Booking.Domain.Repositories;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Repositories
{
    public class PaymentRepository : BaseRepository<PaymentEntity>, IPaymentRepository
    {
        private readonly BookingDbContext _context;

        public PaymentRepository(BookingDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PaymentEntity?> GetByBookingIdAsync(Guid bookingId)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.BookingId == bookingId);
        }

        public async Task<PaymentEntity?> GetByVnpTxnRefAsync(string vnpTxnRef)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.VnpTxnRef == vnpTxnRef);
        }
    }
}
