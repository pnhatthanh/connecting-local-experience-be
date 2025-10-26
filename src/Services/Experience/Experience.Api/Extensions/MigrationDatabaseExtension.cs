using Experience.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Experience.Api.Extensions;

public static class MigrationDatabaseExtension
{
    public static async Task<IServiceProvider> ApplyMigrationAsync(this IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ExperienceDbContext>();
            await dbContext.Database.MigrateAsync();
        }
        return serviceProvider;
    }
}
