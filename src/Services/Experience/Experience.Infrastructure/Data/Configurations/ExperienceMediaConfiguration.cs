using Experience.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Experience.Infrastructure.Data.Configurations
{
    public class ExperienceMediaConfiguration : IEntityTypeConfiguration<ExperienceMediaEntity>
    {
        public void Configure(EntityTypeBuilder<ExperienceMediaEntity> entity)
        {
            entity.ToTable("tbl_experience_media");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_media");
            entity.Property(e => e.ExperienceId).HasColumnName("experience_id").IsRequired();
            entity.Property(e => e.Url).HasColumnName("url").HasMaxLength(500).IsRequired();
            entity.Property(e => e.Order).HasColumnName("order").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(e => e.Experience)
                .WithMany(e => e.Media)
                .HasForeignKey(e => e.ExperienceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ExperienceId);
        }
    }
}
