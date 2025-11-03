using Experience.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Experience.Infrastructure.Data
{
    public class DbInitializer
    {
        public static async Task InitializeAsync(ExperienceDbContext context, ILogger logger)
        {
            try
            {
                if (context.Database.GetPendingMigrations().Any())
                {
                    logger.LogInformation("Applying pending migrations for Experience database...");
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Migrations applied successfully");
                }

                await SeedCategoriesAsync(context, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the Experience database");
                throw;
            }
        }

        private static async Task SeedCategoriesAsync(ExperienceDbContext context, ILogger logger)
        {
            if (await context.ExperienceCategories.AnyAsync())
            {
                logger.LogInformation("Experience categories already seeded");
                return;
            }

            logger.LogInformation("Seeding experience categories...");

            var categories = new List<ExperienceCategoryEntity>
            {
                new ExperienceCategoryEntity
                {
                    Id = Guid.NewGuid(),
                    Name = "Food & Drink",
                    CreatedAt = DateTime.UtcNow
                },
                new ExperienceCategoryEntity
                {
                    Id = Guid.NewGuid(),
                    Name = "Culture & History",
                    CreatedAt = DateTime.UtcNow
                },
                new ExperienceCategoryEntity
                {
                    Id = Guid.NewGuid(),
                    Name = "Nature & Wildlife",
                    CreatedAt = DateTime.UtcNow
                },
                new ExperienceCategoryEntity
                {
                    Id = Guid.NewGuid(),
                    Name = "Sports & Adventure",
                    CreatedAt = DateTime.UtcNow
                },
                new ExperienceCategoryEntity
                {
                    Id = Guid.NewGuid(),
                    Name = "Art & Crafts",
                    CreatedAt = DateTime.UtcNow
                },
                new ExperienceCategoryEntity
                {
                    Id = Guid.NewGuid(),
                    Name = "Music & Entertainment",
                    CreatedAt = DateTime.UtcNow
                },
                new ExperienceCategoryEntity
                {
                    Id = Guid.NewGuid(),
                    Name = "Wellness & Relaxation",
                    CreatedAt = DateTime.UtcNow
                },
                new ExperienceCategoryEntity
                {
                    Id = Guid.NewGuid(),
                    Name = "Nightlife & Social",
                    CreatedAt = DateTime.UtcNow
                },
                new ExperienceCategoryEntity
                {
                    Id = Guid.NewGuid(),
                    Name = "Shopping & Markets",
                    CreatedAt = DateTime.UtcNow
                },
                new ExperienceCategoryEntity
                {
                    Id = Guid.NewGuid(),
                    Name = "Photography & Tours",
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.ExperienceCategories.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            logger.LogInformation($"Seeded {categories.Count} experience categories successfully");
        }
    }
}
