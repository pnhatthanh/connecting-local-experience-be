using BuildingBlocks.EntityFramework;
using Experience.Domain.Entities;
using Experience.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Experience.Infrastructure.Data
{
    public class ExperienceDbContext : BaseDbContext
    {
        public ExperienceDbContext(DbContextOptions<ExperienceDbContext> options) : base(options)
        {
        }

        public DbSet<ExperienceEntity> Experiences { get; set; }
        public DbSet<ExperienceScheduleEntity> ExperienceSchedules { get; set; }
        public DbSet<ExperienceScheduleSlotEntity> ExperienceScheduleSlots { get; set; }
        public DbSet<ExperienceMediaEntity> ExperienceMedia { get; set; }
        public DbSet<ExperienceItineraryEntity> ExperienceItineraries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Experience
            modelBuilder.Entity<ExperienceEntity>(entity =>
            {
                entity.ToTable("tbl_experience");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id_experience");
                entity.Property(e => e.HostId).HasColumnName("host_id").IsRequired();
                entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(255).IsRequired();
                entity.Property(e => e.Description).HasColumnName("description").IsRequired();
                entity.Property(e => e.Location).HasColumnName("location").HasColumnType("geography(Point)").IsRequired();
                entity.Property(e => e.Price).HasColumnName("price").HasColumnType("decimal(10,2)").IsRequired();
                entity.Property(e => e.Duration).HasColumnName("duration").IsRequired();
                entity.Property(e => e.MaxParticipants).HasColumnName("max_participants").IsRequired();
                entity.Property(e => e.Category)
                    .HasColumnName("category")
                    .HasConversion<string>()
                    .IsRequired();
                entity.Property(e => e.Amenities).HasColumnName("amenities").HasColumnType("jsonb");
                entity.Property(e => e.ActivityLevel)
                    .HasColumnName("activity_level")
                    .HasConversion<string>()
                    .IsRequired();
                entity.Property(e => e.SkillLevel)
                    .HasColumnName("skill_level")
                    .HasConversion<string>()
                    .IsRequired();
                entity.Property(e => e.MinAge).HasColumnName("min_age").IsRequired();
                entity.Property(e => e.Accessibility).HasColumnName("accessibility");
                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasConversion<string>()
                    .HasDefaultValue(ExperienceStatus.Draft)
                    .IsRequired();
                entity.Property(e => e.CancellationPolicy).HasColumnName("cancellation_policy").HasMaxLength(255).IsRequired();
                entity.Property(e => e.MeetingPoint).HasColumnName("meeting_point").HasMaxLength(255).IsRequired();
                entity.Property(e => e.Language).HasColumnName("language").HasMaxLength(50).IsRequired();
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(e => e.HostId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Category);
            });

            // Configure ExperienceSchedule
            modelBuilder.Entity<ExperienceScheduleEntity>(entity =>
            {
                entity.ToTable("tbl_experience_schedule");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id_schedule");
                entity.Property(e => e.ExperienceId).HasColumnName("experience_id").IsRequired();
                entity.Property(e => e.IsRecurring).HasColumnName("is_recurring").HasDefaultValue(false);
                entity.Property(e => e.RecurringPattern).HasColumnName("recurring_pattern").HasColumnType("jsonb");
                entity.Property(e => e.StartDate).HasColumnName("start_date");
                entity.Property(e => e.EndDate).HasColumnName("end_date");
                entity.Property(e => e.Timezone).HasColumnName("timezone").HasMaxLength(50).HasDefaultValue("Asia/Ho_Chi_Minh");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(e => e.Experience)
                    .WithMany(e => e.Schedules)
                    .HasForeignKey(e => e.ExperienceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.ExperienceId);
            });

            // Configure ExperienceScheduleSlot
            modelBuilder.Entity<ExperienceScheduleSlotEntity>(entity =>
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
            });

            // Configure ExperienceMedia
            modelBuilder.Entity<ExperienceMediaEntity>(entity =>
            {
                entity.ToTable("tbl_experience_media");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id_media");
                entity.Property(e => e.ExperienceId).HasColumnName("experience_id").IsRequired();
                entity.Property(e => e.Url).HasColumnName("url").HasMaxLength(255).IsRequired();
                entity.Property(e => e.Order).HasColumnName("order").IsRequired();
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(e => e.Experience)
                    .WithMany(e => e.Media)
                    .HasForeignKey(e => e.ExperienceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.ExperienceId);
            });

            // Configure ExperienceItinerary
            modelBuilder.Entity<ExperienceItineraryEntity>(entity =>
            {
                entity.ToTable("tbl_experience_itinerary");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id_itinerary");
                entity.Property(e => e.ExperienceId).HasColumnName("experience_id").IsRequired();
                entity.Property(e => e.StepNumber).HasColumnName("step_number").IsRequired();
                entity.Property(e => e.PhotoUrl).HasColumnName("photo_url").HasMaxLength(255).IsRequired();
                entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(255).IsRequired();
                entity.Property(e => e.Description).HasColumnName("description").IsRequired();
                entity.Property(e => e.Location).HasColumnName("location").HasColumnType("geography(Point)");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(e => e.Experience)
                    .WithMany(e => e.Itineraries)
                    .HasForeignKey(e => e.ExperienceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.ExperienceId);
            });
        }
    }
}
