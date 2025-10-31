using Experience.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Experience.Infrastructure.Data.Configurations
{
    public class ExperienceCategoryConfiguration : IEntityTypeConfiguration<ExperienceCategoryEntity>
    {
        public void Configure(EntityTypeBuilder<ExperienceCategoryEntity> entity)
        {
            entity.ToTable("tbl_experience_category");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_category");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(e => e.Name).IsUnique();

        }
    }
}
