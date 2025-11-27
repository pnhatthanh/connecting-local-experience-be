using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Data.Configurations
{
    public class RefundConfiguration : IEntityTypeConfiguration<RefundEntity>
    {
        public void Configure(EntityTypeBuilder<RefundEntity> entity)
        {
            entity.ToTable("refunds");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.RefundAmount)
                .HasPrecision(10, 2);
            
            entity.Property(e => e.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("VND");
            
            entity.Property(e => e.Reason)
                .HasMaxLength(500);
            
            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(e => e.VnpRefundRef)
                .HasMaxLength(100);

            entity.Property(e => e.VnpResponseCode)
                .HasMaxLength(10);

            entity.HasIndex(e => e.PaymentId);
            entity.HasIndex(e => e.BookingId);
            entity.HasIndex(e => e.Status);

            // Relationships
            entity.HasOne(e => e.Booking)
                .WithMany(b => b.Refunds)
                .HasForeignKey(e => e.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Payment)
                .WithMany(p => p.Refunds)
                .HasForeignKey(e => e.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
