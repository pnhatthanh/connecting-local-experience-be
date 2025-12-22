using Experience.Domain.Entities;
using Experience.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Experience.Infrastructure.Data.Configurations
{
    public class ExperienceScheduleSlotConfiguration : IEntityTypeConfiguration<ExperienceScheduleSlotEntity>
    {
        public void Configure(EntityTypeBuilder<ExperienceScheduleSlotEntity> entity)
        {
            entity.ToTable("tbl_experience_schedule_slot");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_slot");
            entity.Property(e => e.ScheduleId).HasColumnName("schedule_id").IsRequired();
            entity.Property(e => e.Date).HasColumnName("date").IsRequired();
            entity.Property(e => e.StartTime).HasColumnName("start_time").IsRequired();
            entity.Property(e => e.EndTime).HasColumnName("end_time").IsRequired();
            entity.Property(e => e.TotalSlots).HasColumnName("total_slots").IsRequired();
            entity.Property(e => e.AvailableSlots).HasColumnName("available_slots").IsRequired();
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasDefaultValue(SlotStatus.Open);
            entity.Property(e => e.CancelReason).HasColumnName("cancel_reason");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(e => e.Schedule)
                .WithMany(e => e.Slots)
                .HasForeignKey(e => e.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ScheduleId);
            entity.HasIndex(e => e.Date);
            entity.HasIndex(e => e.Status);
        }
    }
}
