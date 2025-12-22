using Experience.Domain.Entities;
using Experience.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Experience.Infrastructure.Data.Configurations
{
    public class ExperienceConfiguration : IEntityTypeConfiguration<ExperienceEntity>
    {
        public void Configure(EntityTypeBuilder<ExperienceEntity> entity)
        {
            entity.ToTable("tbl_experience");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_experience");
            entity.Property(e => e.HostId).HasColumnName("host_id").IsRequired();
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(255).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").IsRequired();
            entity.Property(e => e.Address).HasColumnName("address").HasMaxLength(500).IsRequired();
            entity.Property(e => e.District).HasColumnName("district").HasMaxLength(100).IsRequired();
            entity.Property(e => e.City).HasColumnName("city").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Country).HasColumnName("country").HasMaxLength(100).IsRequired();
            entity.Property(e => e.AdultPrice).HasColumnName("adult_price").HasColumnType("decimal(10,2)").IsRequired();
            entity.Property(e => e.ChildPrice).HasColumnName("child_price").HasColumnType("decimal(10,2)").IsRequired();
            entity.Property(e => e.Duration).HasColumnName("duration").IsRequired();
            entity.Property(e => e.MaxParticipants).HasColumnName("max_participants").IsRequired();
            entity.Property(e => e.CategoryId).HasColumnName("category_id").IsRequired();
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
            entity.Property(e => e.CancellationPolicy)
                .HasColumnName("cancellation_policy")
                .HasConversion<string>()
                .HasDefaultValue(CancellationPolicyType.AlwaysFreeCancellation)
                .IsRequired();
            entity.Property(e => e.Language).HasColumnName("language").HasMaxLength(50).IsRequired();
            entity.Property(e => e.TotalReviews).HasColumnName("total_reviews").HasDefaultValue(0).IsRequired();
            entity.Property(e => e.AverageRating).HasColumnName("average_rating").HasColumnType("double precision").HasDefaultValue(0.0).IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(e => e.Category)
                .WithMany(c => c.Experiences)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Schedule)
                .WithOne(s => s.Experience)
                .HasForeignKey<ExperienceScheduleEntity>(s => s.ExperienceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Media relationship with cascade delete for orphans
            entity.HasMany(e => e.Media)
                .WithOne(m => m.Experience)
                .HasForeignKey(m => m.ExperienceId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Configure Itineraries relationship with cascade delete for orphans
            entity.HasMany(e => e.Itineraries)
                .WithOne(i => i.Experience)
                .HasForeignKey(i => i.ExperienceId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Configure Reviews relationship with cascade delete
            entity.HasMany(e => e.Reviews)
                .WithOne(r => r.Experience)
                .HasForeignKey(r => r.ExperienceId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            entity.HasIndex(e => e.HostId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CategoryId);
        }
    }
}
