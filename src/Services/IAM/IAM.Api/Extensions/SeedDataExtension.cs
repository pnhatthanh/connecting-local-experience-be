using IAM.Application.Interfaces;
using IAM.Domain.Entities;
using IAM.Domain.Enums;
using IAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using PermissionCodesShared = BuildingBlocks.Presentation.Constants.PermissionCodes;

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
            
            // Seed Permissions first
            await SeedPermissionsAsync(dbContext, logger);
            
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

            await SeedRolePermissionsAsync(dbContext, logger);
            await SeedAdminAccountAsync(dbContext, passwordHasher, logger);
        }
        return serviceProvider;
    }

    private static async Task SeedPermissionsAsync(IAMDbContext context, ILogger logger)
    {
        if (await context.Set<PermissionEntity>().AnyAsync())
        {
            logger.LogInformation("Permissions already seeded, skipping");
            return;
        }

        // Get all permission codes from BuildingBlocks.Presentation.Constants.PermissionCodes using reflection
        var permissionCodesType = typeof(PermissionCodesShared);
        var permissionFields = permissionCodesType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy)
            .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string));

        var permissions = permissionFields.Select(f => new PermissionEntity
        {
            Id = Guid.NewGuid(),
            PermissionCode = f.GetValue(null)?.ToString() ?? string.Empty,
            Name = f.Name.Replace("_", " "),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }).ToList();

        await context.Set<PermissionEntity>().AddRangeAsync(permissions);
        await context.SaveChangesAsync();

        logger.LogInformation("========================================");
        logger.LogInformation($"Seeded {permissions.Count} permissions successfully!");
        logger.LogInformation("========================================");
    }

    private static async Task SeedRolePermissionsAsync(IAMDbContext context, ILogger logger)
    {
        if (await context.Set<RolePermissionEntity>().AnyAsync())
        {
            logger.LogInformation("Role-Permissions already seeded, skipping");
            return;
        }

        var roles = await context.Roles.ToListAsync();
        var permissions = await context.Set<PermissionEntity>().ToListAsync();

        var userRole = roles.FirstOrDefault(r => r.Name == AccountRole.User);
        var hostRole = roles.FirstOrDefault(r => r.Name == AccountRole.Host);
        var adminRole = roles.FirstOrDefault(r => r.Name == AccountRole.Admin);

        var rolePermissions = new List<RolePermissionEntity>();

        // Admin có tất cả permissions
        if (adminRole != null)
        {
            rolePermissions.AddRange(permissions.Select(p => new RolePermissionEntity
            {
                RoleId = adminRole.Id,
                PermissionId = p.Id,
                Licensed = true
            }));
        }

        if (userRole != null)
        {
            var userPermissionCodes = new[]
            {
                PermissionCodesShared.USER_USER_BECOME_HOST,
                PermissionCodesShared.BOOKING_BOOKING_CREATE,
                PermissionCodesShared.BOOKING_BOOKING_CANCEL,
            };

            rolePermissions.AddRange(permissions
                .Where(p => userPermissionCodes.Contains(p.PermissionCode))
                .Select(p => new RolePermissionEntity
                {
                    RoleId = userRole.Id,
                    PermissionId = p.Id,
                    Licensed = true
                }));
        }

        if (hostRole != null)
        {
            var hostPermissionCodes = new[]
            {
                PermissionCodesShared.BOOKING_BOOKING_CREATE,
                PermissionCodesShared.BOOKING_BOOKING_CANCEL,
                PermissionCodesShared.BOOKING_BOOKING_CHECK_COMPLETED,
                PermissionCodesShared.EXPERIENCE_EXPERIENCE_CREATE,
                PermissionCodesShared.EXPERIENCE_EXPERIENCE_UPDATE,
                PermissionCodesShared.EXPERIENCE_EXPERIENCE_DELETE,
                PermissionCodesShared.BOOKING_BOOKING_VIEW_BY_HOST,
                PermissionCodesShared.BOOKING_BOOKING_TOGGLE_STATUS
            };

            rolePermissions.AddRange(permissions
                .Where(p => hostPermissionCodes.Contains(p.PermissionCode))
                .Select(p => new RolePermissionEntity
                {
                    RoleId = hostRole.Id,
                    PermissionId = p.Id,
                    Licensed = true
                }));
        }

        await context.Set<RolePermissionEntity>().AddRangeAsync(rolePermissions);
        await context.SaveChangesAsync();
        logger.LogInformation($"Seeded {rolePermissions.Count} role-permission mappings successfully!");
    }

    private static async Task SeedAdminAccountAsync(IAMDbContext context, IPasswordHasher passwordHasher, ILogger logger)
    {
        const string adminEmail = "admin@gmail.com";

        if (await context.Accounts.AnyAsync(a => a.Email == adminEmail))
        {
            logger.LogInformation("Admin account already exists, skipping seed");
            return;
        }
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
        logger.LogInformation("Admin account seeded successfully!");
    }
}
