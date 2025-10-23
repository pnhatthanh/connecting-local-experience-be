using BuildingBlocks.EntityFramework;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Data
{
    public class UserDbContext : BaseDbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        public DbSet<ProfileEntity> Profiles { get; set; }
        public DbSet<AccountEntity> Accounts { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProfileEntity>(entity =>
            {
                entity.ToTable("profiles");
                
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                
                entity.Property(e => e.AccountId).HasColumnName("account_id").IsRequired();
                entity.Property(e => e.FullName).HasColumnName("full_name").HasMaxLength(255).IsRequired();
                entity.Property(e => e.PhoneNumber).HasColumnName("phone_number").HasMaxLength(20);
                entity.Property(e => e.Nationality).HasColumnName("nationality").HasMaxLength(100);
                entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url").HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                // Indexes
                entity.HasIndex(e => e.AccountId).IsUnique();
            });

            modelBuilder.Entity<AccountEntity>(entity =>
            {
                entity.ToTable("tbl_account");
                
                entity.HasKey(e => e.IdAccount);
                entity.Property(e => e.IdAccount).HasColumnName("id_account");
                
                entity.Property(e => e.FullName).HasColumnName("full_name").HasMaxLength(255);
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash").IsRequired();
                entity.Property(e => e.IsEmailConfirmed).HasColumnName("is_email_confirmed");
                entity.Property(e => e.EmailConfirmationToken).HasColumnName("email_confirmation_token");
                entity.Property(e => e.PasswordResetToken).HasColumnName("password_reset_token");
                entity.Property(e => e.PasswordResetTokenExpiry).HasColumnName("password_reset_token_expiry");
                entity.Property(e => e.LastLoginAt).HasColumnName("last_login_at");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.RoleId).HasColumnName("role_id");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<RoleEntity>(entity =>
            {
                entity.ToTable("tbl_role");
                
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id_role");
                
                entity.Property(e => e.Name).HasColumnName("name").IsRequired();
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });
        }
    }
}
