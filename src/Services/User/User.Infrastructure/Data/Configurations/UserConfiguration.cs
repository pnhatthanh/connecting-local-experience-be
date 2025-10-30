using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User.Domain.Entities;
using User.Domain.Enums;

namespace User.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.ToTable("tbl_users");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
            builder.Property(e => e.PhoneNumber).HasColumnName("phone_number").HasMaxLength(50);
            builder.Property(e => e.FullName).HasColumnName("full_name").HasMaxLength(255).IsRequired();
            builder.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            builder.Property(e => e.Gender)
                .HasColumnName("gender")
                .HasConversion(
                    v => v.HasValue ? v.Value.ToString().ToLower() : null,
                    v => v != null ? Enum.Parse<Gender>(v, true) : null
                );
            builder.Property(e => e.AvatarUrl).HasColumnName("avatar_url").HasMaxLength(500);
            builder.Property(e => e.Country).HasColumnName("country").HasMaxLength(100);
            builder.Property(e => e.Role)
                .HasColumnName("role")
                .HasConversion(
                    v => v.ToString().ToLower(),
                    v => Enum.Parse<UserRole>(v, true)
                )
                .HasDefaultValue(UserRole.User);
            builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            builder.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            builder.HasIndex(e => e.Email).IsUnique();

            builder.HasOne(e => e.HostProfile)
                .WithOne(e => e.User)
                .HasForeignKey<HostProfileEntity>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.FavoriteExperiences)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
