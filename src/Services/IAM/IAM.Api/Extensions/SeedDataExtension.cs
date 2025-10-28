using IAM.Domain.Entities;
using IAM.Domain.Enums;
using IAM.Infrastructure.Data;

namespace IAM.Api.Extensions;

public static class SeedDataExtension
{
    public static async Task<IServiceProvider> SeedDataAsync(this IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<IAMDbContext>();
            
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
                        Name = AccountRole.Provider,
                        Description = "Experience provider role",
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
            }
        }
        return serviceProvider;
    }
}
