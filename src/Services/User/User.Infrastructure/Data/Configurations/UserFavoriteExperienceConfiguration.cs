using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User.Domain.Entities;

namespace User.Infrastructure.Data.Configurations
{
    public class UserFavoriteExperienceConfiguration : IEntityTypeConfiguration<UserFavoriteExperienceEntity>
    {
        public void Configure(EntityTypeBuilder<UserFavoriteExperienceEntity> builder)
        {
            builder.ToTable("tbl_user_favorite_experiences");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            builder.Property(e => e.ExperienceId).HasColumnName("experience_id").IsRequired();
            builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");

            builder.HasIndex(e => new { e.UserId, e.ExperienceId }).IsUnique();
            builder.HasIndex(e => e.ExperienceId);
        }
    }
}
