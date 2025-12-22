using Experience.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Experience.Infrastructure.Data.Configurations
{
    public class ExperienceScheduleConfiguration : IEntityTypeConfiguration<ExperienceScheduleEntity>
    {
        public void Configure(EntityTypeBuilder<ExperienceScheduleEntity> entity)
        {
            entity.ToTable("tbl_experience_schedule");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_schedule");
            entity.Property(e => e.ExperienceId).HasColumnName("experience_id").IsRequired();
            entity.Property(e => e.RecurrenceType).HasColumnName("recurrence_type").IsRequired();
            
            entity.Property(e => e.DaysOfWeek)
                .HasColumnName("days_of_week")
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                    v => JsonSerializer.Deserialize<List<DayOfWeek>>(v, (JsonSerializerOptions)null!) ?? new List<DayOfWeek>()
                );
            
            entity.Property(e => e.TimeSlots)
                .HasColumnName("time_slots")
                .HasColumnType("jsonb")
                .IsRequired()
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                    v => JsonSerializer.Deserialize<List<ScheduleTimeSlot>>(v, (JsonSerializerOptions)null!) ?? new List<ScheduleTimeSlot>()
                );
            
            entity.Property(e => e.StartDate).HasColumnName("start_date").IsRequired();
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(e => e.ExperienceId).IsUnique();
        }
    }
}
