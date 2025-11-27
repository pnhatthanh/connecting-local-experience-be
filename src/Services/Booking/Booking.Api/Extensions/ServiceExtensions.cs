using BuildingBlocks.RabbitMQ;
using BuildingBlocks.RabbitMQ.Configurations;
using BuildingBlocks.Application.EventBus.Abstractions;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBookingRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMQSetting = configuration.GetSection("RabbitMQ").Get<RabbitMQConfig>()
                ?? throw new ArgumentNullException("RabbitMQ configuration is null");
            services.AddRabbitMQ(rabbitMQSetting);
            
            return services;
        }

        public static async Task<IApplicationBuilder> SubscribeToEvents(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
            return app;
        }

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
