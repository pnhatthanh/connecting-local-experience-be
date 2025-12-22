using Experience.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Experience.Api.Extensions;

public static class MigrationDatabaseExtension
{
    public static async Task<IServiceProvider> ApplyMigrationAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ExperienceDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ExperienceDbContext>>();
        
        try
        {
            await DbInitializer.InitializeAsync(dbContext, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the Experience database");
            throw;
        }
        
        return serviceProvider;
    }
}
