using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Data.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<AccountEntity>
    {
        public void Configure(EntityTypeBuilder<AccountEntity> builder)
        {
            builder.ToTable("tbl_account");

            builder.HasKey(e => e.IdAccount);
            builder.Property(e => e.IdAccount)
                .HasColumnName("id_account")
                .IsRequired();

            builder.Property(e => e.FullName)
                .HasColumnName("full_name")
                .HasMaxLength(255);

            builder.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(e => e.PasswordHash)
                .HasColumnName("password_hash")
                .IsRequired();

            builder.Property(e => e.IsEmailConfirmed)
                .HasColumnName("is_email_confirmed")
                .IsRequired();

            builder.Property(e => e.EmailConfirmationToken)
                .HasColumnName("email_confirmation_token");

            builder.Property(e => e.PasswordResetToken)
                .HasColumnName("password_reset_token");

            builder.Property(e => e.PasswordResetTokenExpiry)
                .HasColumnName("password_reset_token_expiry");

            builder.Property(e => e.LastLoginAt)
                .HasColumnName("last_login_at");

            builder.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            builder.Property(e => e.RoleId)
                .HasColumnName("role_id");

            builder.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");
        }
    }
}
