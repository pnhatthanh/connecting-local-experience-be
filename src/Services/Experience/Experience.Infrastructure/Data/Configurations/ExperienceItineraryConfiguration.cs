using Experience.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Experience.Infrastructure.Data.Configurations
{
    public class ExperienceItineraryConfiguration : IEntityTypeConfiguration<ExperienceItineraryEntity>
    {
        public void Configure(EntityTypeBuilder<ExperienceItineraryEntity> entity)
        {
            entity.ToTable("tbl_experience_itinerary");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id_itinerary");
            entity.Property(e => e.ExperienceId).HasColumnName("experience_id").IsRequired();
            entity.Property(e => e.StepNumber).HasColumnName("step_number").IsRequired();
            entity.Property(e => e.PhotoUrl).HasColumnName("photo_url").HasMaxLength(500).IsRequired();
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(255).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(e => e.ExperienceId);
        }
    }
}
