using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Data.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<PaymentEntity>
    {
        public void Configure(EntityTypeBuilder<PaymentEntity> entity)
        {
            entity.ToTable("payments");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2);
            
            entity.Property(e => e.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("VND");
            
            entity.Property(e => e.Provider)
                .IsRequired()
                .HasConversion<string>();
            
            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(e => e.TransactionId)
                .HasMaxLength(100);

            entity.Property(e => e.PaymentUrl)
                .HasMaxLength(2000);

            entity.HasIndex(e => e.BookingId);
            entity.HasIndex(e => e.TransactionId);

            // Relationship
            entity.HasOne(e => e.Booking)
                .WithOne(b => b.Payment)
                .HasForeignKey<PaymentEntity>(e => e.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
