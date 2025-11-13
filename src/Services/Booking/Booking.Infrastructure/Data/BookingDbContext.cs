using BuildingBlocks.EntityFramework;
using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Data
{
    public class BookingDbContext : BaseDbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
        {
        }

        public DbSet<BookingEntity> Bookings { get; set; } = null!;
        public DbSet<PaymentEntity> Payments { get; set; } = null!;
        public DbSet<RefundEntity> Refunds { get; set; } = null!;
        public DbSet<BookingCancellationEntity> BookingCancellations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Booking Entity Configuration
            modelBuilder.Entity<BookingEntity>(entity =>
            {
                entity.ToTable("bookings");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.BookingCode)
                    .IsRequired()
                    .HasMaxLength(20);
                
                entity.HasIndex(e => e.BookingCode).IsUnique();
                
                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion<string>();
                
                entity.Property(e => e.ContactName)
                    .IsRequired()
                    .HasMaxLength(255);
                
                entity.Property(e => e.ContactEmail)
                    .IsRequired()
                    .HasMaxLength(255);
                
                entity.Property(e => e.ContactPhone)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(e => e.TotalPrice)
                    .HasPrecision(10, 2);

                entity.HasOne(e => e.Payment)
                    .WithOne(p => p.Booking)
                    .HasForeignKey<PaymentEntity>(p => p.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Cancellation)
                    .WithOne(c => c.Booking)
                    .HasForeignKey<BookingCancellationEntity>(c => c.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Refunds)
                    .WithOne(r => r.Booking)
                    .HasForeignKey(r => r.BookingId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Payment Entity Configuration
            modelBuilder.Entity<PaymentEntity>(entity =>
            {
                entity.ToTable("payments");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Amount)
                    .HasPrecision(10, 2);
                
                entity.Property(e => e.Currency)
                    .HasMaxLength(3)
                    .HasDefaultValue("VND");
                
                entity.Property(e => e.Provider)
                    .IsRequired()
                    .HasConversion<string>();
                
                entity.Property(e => e.Method)
                    .HasConversion<string>();
                
                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion<string>();
                
                entity.Property(e => e.VnpTxnRef)
                    .HasMaxLength(100);
                
                entity.HasIndex(e => e.VnpTxnRef).IsUnique();
                
                entity.Property(e => e.VnpResponseCode)
                    .HasMaxLength(10);
                
                entity.Property(e => e.VnpBankCode)
                    .HasMaxLength(50);
                
                entity.Property(e => e.VnpSecureHash)
                    .HasMaxLength(255);

                entity.HasMany(e => e.Refunds)
                    .WithOne(r => r.Payment)
                    .HasForeignKey(r => r.PaymentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Refund Entity Configuration
            modelBuilder.Entity<RefundEntity>(entity =>
            {
                entity.ToTable("refunds");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.RefundAmount)
                    .HasPrecision(10, 2);
                
                entity.Property(e => e.Currency)
                    .HasMaxLength(3)
                    .HasDefaultValue("VND");
                
                entity.Property(e => e.Reason)
                    .IsRequired();
                
                entity.Property(e => e.VnpRefundRef)
                    .HasMaxLength(100);
                
                entity.HasIndex(e => e.VnpRefundRef).IsUnique();
                
                entity.Property(e => e.VnpResponseCode)
                    .HasMaxLength(10);
                
                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion<string>();
            });

            // BookingCancellation Entity Configuration
            modelBuilder.Entity<BookingCancellationEntity>(entity =>
            {
                entity.ToTable("booking_cancellations");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.CancelledBy)
                    .IsRequired()
                    .HasConversion<string>();
                
                entity.Property(e => e.Reason)
                    .IsRequired();
                
                entity.Property(e => e.CancellationFee)
                    .HasPrecision(10, 2)
                    .HasDefaultValue(0);
                
                entity.Property(e => e.RefundAmount)
                    .HasPrecision(10, 2)
                    .HasDefaultValue(0);
            });
        }
    }
}
