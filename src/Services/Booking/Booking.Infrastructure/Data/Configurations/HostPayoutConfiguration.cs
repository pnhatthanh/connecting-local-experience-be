using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Data.Configurations
{
    public class HostPayoutConfiguration : IEntityTypeConfiguration<HostPayoutEntity>
    {
        public void Configure(EntityTypeBuilder<HostPayoutEntity> entity)
        {
            entity.ToTable("host_payouts");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2);
            
            entity.Property(e => e.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("VND");
            
            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(e => e.TransactionReference)
                .HasMaxLength(100);

            entity.HasIndex(e => e.BookingId).IsUnique();
            entity.HasIndex(e => e.HostId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.ScheduledDate);

            // Relationship
            entity.HasOne(e => e.Booking)
                .WithOne(b => b.HostPayout)
                .HasForeignKey<HostPayoutEntity>(e => e.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
