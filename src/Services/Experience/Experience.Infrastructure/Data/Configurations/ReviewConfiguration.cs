using Experience.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Experience.Infrastructure.Data.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<ReviewEntity>
    {
        public void Configure(EntityTypeBuilder<ReviewEntity> entity)
        {
            entity.ToTable("tbl_review");
            
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Id).HasColumnName("id_review");
            
            entity.Property(r => r.ExperienceId).HasColumnName("experience_id").IsRequired();
            entity.Property(r => r.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(r => r.Rating).HasColumnName("rating").IsRequired();
            entity.Property(r => r.Description).HasColumnName("description").HasMaxLength(1000).IsRequired();
            entity.Property(r => r.IsHidden).HasColumnName("is_hidden").HasDefaultValue(false).IsRequired();
            entity.Property(r => r.CreatedAt).HasColumnName("created_at");
            entity.Property(r => r.UpdatedAt).HasColumnName("updated_at");

            // Configure relationship with Experience
            entity.HasOne(r => r.Experience)
                .WithMany(e => e.Reviews)
                .HasForeignKey(r => r.ExperienceId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}
