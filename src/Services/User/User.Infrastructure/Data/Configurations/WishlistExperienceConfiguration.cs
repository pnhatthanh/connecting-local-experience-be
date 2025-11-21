using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User.Domain.Entities;

namespace User.Infrastructure.Data.Configurations
{
    public class WishlistExperienceConfiguration : IEntityTypeConfiguration<WishlistExperienceEntity>
    {
        public void Configure(EntityTypeBuilder<WishlistExperienceEntity> builder)
        {
            builder.ToTable("tbl_wishlist_experiences");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.WishlistId).HasColumnName("wishlist_id").IsRequired();
            builder.Property(e => e.ExperienceId).HasColumnName("experience_id").IsRequired();
            builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            builder.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            builder.HasOne(e => e.Wishlist)
                .WithMany(w => w.WishlistExperiences)
                .HasForeignKey(e => e.WishlistId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => new { e.WishlistId, e.ExperienceId }).IsUnique();
            builder.HasIndex(e => e.WishlistId);
            builder.HasIndex(e => e.ExperienceId);
        }
    }
}
