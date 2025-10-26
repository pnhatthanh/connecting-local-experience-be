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

            var now = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            entity.HasData(
                new ExperienceCategoryEntity { Id = Guid.NewGuid(), Name = "Arts", CreatedAt = now, UpdatedAt = now },
                new ExperienceCategoryEntity { Id = Guid.NewGuid(), Name = "Cooking", CreatedAt = now, UpdatedAt = now },
                new ExperienceCategoryEntity { Id = Guid.NewGuid(), Name = "Food Drink", CreatedAt = now, UpdatedAt = now },
                new ExperienceCategoryEntity { Id = Guid.NewGuid(), Name = "Nature", CreatedAt = now, UpdatedAt = now },
                new ExperienceCategoryEntity { Id = Guid.NewGuid(), Name = "Sports", CreatedAt = now, UpdatedAt = now },
                new ExperienceCategoryEntity { Id = Guid.NewGuid(), Name = "History", CreatedAt = now, UpdatedAt = now },
                new ExperienceCategoryEntity { Id = Guid.NewGuid(), Name = "Music", CreatedAt = now, UpdatedAt = now },
                new ExperienceCategoryEntity { Id = Guid.NewGuid(), Name = "Nightlife", CreatedAt = now, UpdatedAt = now },
                new ExperienceCategoryEntity { Id = Guid.NewGuid(), Name = "Wellness", CreatedAt = now, UpdatedAt = now },
                new ExperienceCategoryEntity { Id = Guid.NewGuid(), Name = "Social Impact", CreatedAt = now, UpdatedAt = now },
                new ExperienceCategoryEntity { Id = Guid.NewGuid(), Name = "Photography", CreatedAt = now, UpdatedAt = now },
                new ExperienceCategoryEntity { Id = Guid.NewGuid(), Name = "Animal", CreatedAt = now, UpdatedAt = now },
                new ExperienceCategoryEntity { Id = Guid.NewGuid(), Name = "Entertainment", CreatedAt = now, UpdatedAt = now }
            );
        }
    }
}
