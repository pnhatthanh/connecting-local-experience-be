using BuildingBlocks.EntityFramework;
using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Data
{
    public class BookingDbContext(DbContextOptions<BookingDbContext> options) 
        : BaseDbContext(options)
    {
        public DbSet<BookingEntity> Bookings { get; set; }
        public DbSet<PaymentEntity> Payments { get; set; }
        public DbSet<RefundEntity> Refunds { get; set; }
        public DbSet<BookingCancellationEntity> BookingCancellations { get; set; }
        public DbSet<HostPayoutEntity> HostPayouts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookingDbContext).Assembly);
        }
    }
}
