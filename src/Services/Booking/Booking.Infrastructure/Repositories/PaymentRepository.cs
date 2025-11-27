using BuildingBlocks.EntityFramework;
using Booking.Domain.Entities;
using Booking.Domain.Repositories;
using Booking.Infrastructure.Data;

namespace Booking.Infrastructure.Repositories
{
    public class PaymentRepository(BookingDbContext context) : BaseRepository<PaymentEntity>(context), IPaymentRepository
    {
    }
}
