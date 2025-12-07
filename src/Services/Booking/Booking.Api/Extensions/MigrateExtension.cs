using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Api.Extensions
{
    public static class MigrateExtension
    {
        public static async Task<IServiceProvider> ApplyMigrationAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
            if ((await dbContext.Database.GetPendingMigrationsAsync()).Any())
            {
                await dbContext.Database.MigrateAsync();
            }
            return serviceProvider;
        }
    }
}
