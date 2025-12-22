using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Data.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<BookingEntity>
    {
        public void Configure(EntityTypeBuilder<BookingEntity> entity)
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
            
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(255);
            
            entity.Property(e => e.LastName)
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
            
            entity.Property(e => e.ExperienceTitle)
                .IsRequired()
                .HasMaxLength(500);
            
            entity.Property(e => e.ImageUrl)
                .IsRequired()
                .HasMaxLength(1000);

            // Indexes
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.HostId);
            entity.HasIndex(e => e.ExperienceId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Date);
        }
    }
}
