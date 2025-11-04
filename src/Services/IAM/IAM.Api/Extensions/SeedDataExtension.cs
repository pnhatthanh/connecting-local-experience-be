using IAM.Application.Interfaces;
using IAM.Domain.Entities;
using IAM.Domain.Enums;
using IAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IAM.Api.Extensions;

public static class SeedDataExtension
{
    public static async Task<IServiceProvider> SeedDataAsync(this IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<IAMDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            
            // Seed Roles if not exists
            if (!dbContext.Roles.Any())
            {
                var roles = new List<RoleEntity>
                {
                    new RoleEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = AccountRole.User,
                        Description = "Regular user role",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new RoleEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = AccountRole.Host,
                        Description = "Experience host role",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new RoleEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = AccountRole.Admin,
                        Description = "Administrator role",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                await dbContext.Roles.AddRangeAsync(roles);
                await dbContext.SaveChangesAsync();
                
                logger.LogInformation("Roles seeded successfully");
            }

            await SeedAdminAccountAsync(dbContext, passwordHasher, logger);
        }
        return serviceProvider;
    }

    private static async Task SeedAdminAccountAsync(IAMDbContext context, IPasswordHasher passwordHasher, ILogger logger)
    {
        const string adminEmail = "admin@gmail.com";

        // Check if admin account already exists
        if (await context.Accounts.AnyAsync(a => a.Email == adminEmail))
        {
            logger.LogInformation("Admin account already exists, skipping seed");
            return;
        }

        // Get Admin role
        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == AccountRole.Admin);
        if (adminRole == null)
        {
            logger.LogWarning("Admin role not found, cannot seed admin account");
            return;
        }

        // Create admin account
        var adminAccount = new AccountEntity
        {
            Id = Guid.NewGuid(),
            Email = adminEmail,
            FullName = "System Administrator",
            PasswordHash = passwordHasher.HashPassword("Admin123@"),
            RoleId = adminRole.Id,
            IsEmailConfirmed = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.Accounts.AddAsync(adminAccount);
        await context.SaveChangesAsync();

        logger.LogInformation("========================================");
        logger.LogInformation("Admin account seeded successfully!");
        logger.LogInformation("Email: {Email}", adminEmail);
        logger.LogInformation("Password: Admin@123456");
        logger.LogInformation("========================================");
    }
}
