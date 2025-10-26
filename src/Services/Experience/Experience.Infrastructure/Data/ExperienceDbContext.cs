using BuildingBlocks.EntityFramework;
using Experience.Domain.Entities;
using Experience.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Experience.Infrastructure.Data
{
    public class ExperienceDbContext(DbContextOptions<ExperienceDbContext> options) 
        : BaseDbContext(options)
    {
        public DbSet<ExperienceEntity> Experiences { get; set; }
        public DbSet<ExperienceCategoryEntity> ExperienceCategories { get; set; }
        public DbSet<ExperienceScheduleEntity> ExperienceSchedules { get; set; }
        public DbSet<ExperienceScheduleSlotEntity> ExperienceScheduleSlots { get; set; }
        public DbSet<ExperienceMediaEntity> ExperienceMedias { get; set; }
        public DbSet<ExperienceItineraryEntity> ExperienceItineraries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ExperienceDbContext).Assembly);
        }
    }
}
