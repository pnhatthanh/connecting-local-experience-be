using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using BuildingBlocks.Domain.Interfaces;

namespace BuildingBlocks.EntityFramework
{
    public static class AddDbContext
    {
        public static IServiceCollection AddDbContextSqlServer<T>(this IServiceCollection services, string connectionString)
            where T : BaseDbContext
        {
            services.AddDbContext<T>(options =>
                options.UseNpgsql(connectionString));
            return services;
        }
        public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection services)
           where TContext : BaseDbContext
        {
            services.AddScoped<IUnitOfWork, UnitOfWork<TContext>>();
            return services;
        }
        public static async Task ApplyMigrationsAsync<T>(this IServiceProvider serviceProvider)
            where T : BaseDbContext
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<T>();
            try
            {
                await context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while applying migrations", ex);
            }
        }
    }
}
