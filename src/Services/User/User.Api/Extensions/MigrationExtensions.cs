using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using User.Infrastructure.Data;

namespace User.Api.Extensions
{
    public static class MigrationExtensions
    {
        public static async Task ApplyMigrationAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
            await dbContext.Database.MigrateAsync();
        }
    }
}
