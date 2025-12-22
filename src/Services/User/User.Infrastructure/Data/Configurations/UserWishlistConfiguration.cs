using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User.Domain.Entities;

namespace User.Infrastructure.Data.Configurations
{
    public class UserWishlistConfiguration : IEntityTypeConfiguration<UserWishlistEntity>
    {
        public void Configure(EntityTypeBuilder<UserWishlistEntity> builder)
        {
            builder.ToTable("tbl_user_wishlists");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            builder.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
            builder.Property(e => e.ExperienceCount).HasColumnName("experience_count").HasDefaultValue(0);
            builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            builder.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            builder.HasOne(e => e.User)
                .WithMany(u => u.Wishlists)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => new { e.UserId, e.Name }).IsUnique();
            builder.HasIndex(e => e.UserId);
        }
    }
}
