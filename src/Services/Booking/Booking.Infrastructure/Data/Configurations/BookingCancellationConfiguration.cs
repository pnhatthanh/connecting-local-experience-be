using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Data.Configurations
{
    public class BookingCancellationConfiguration : IEntityTypeConfiguration<BookingCancellationEntity>
    {
        public void Configure(EntityTypeBuilder<BookingCancellationEntity> entity)
        {
            entity.ToTable("booking_cancellations");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.CancelledBy)
                .IsRequired()
                .HasConversion<string>();
            
            entity.Property(e => e.Reason)
                .IsRequired()
                .HasMaxLength(500);
            
            entity.Property(e => e.CancellationFee)
                .HasPrecision(10, 2);
            
            entity.Property(e => e.RefundAmount)
                .HasPrecision(10, 2);

            entity.HasIndex(e => e.BookingId).IsUnique();
            entity.HasIndex(e => e.CancelledBy);

            // Relationship
            entity.HasOne(e => e.Booking)
                .WithOne(b => b.Cancellation)
                .HasForeignKey<BookingCancellationEntity>(e => e.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
