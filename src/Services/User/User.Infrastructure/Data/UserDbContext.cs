using BuildingBlocks.EntityFramework;
using Microsoft.EntityFrameworkCore;
using User.Domain.Entities;

namespace User.Infrastructure.Data
{
    public class UserDbContext : BaseDbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) 
            : base(options)
        {}
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<HostProfileEntity> HostProfiles { get; set; }
        public DbSet<UserWishlistEntity> UserWishlists { get; set; }
        public DbSet<WishlistExperienceEntity> WishlistExperiences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserDbContext).Assembly);
        }
    }
}