using BuildingBlocks.EntityFramework;
using Booking.Domain.Entities;
using Booking.Domain.Repositories;
using Booking.Infrastructure.Data;

namespace Booking.Infrastructure.Repositories
{
    public class RefundRepository(BookingDbContext context) : BaseRepository<RefundEntity>(context), IRefundRepository
    {
    }
}
