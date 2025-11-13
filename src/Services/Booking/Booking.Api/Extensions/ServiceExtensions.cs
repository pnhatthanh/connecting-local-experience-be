using BuildingBlocks.RabbitMQ;
using BuildingBlocks.RabbitMQ.Configurations;
using BuildingBlocks.Application.EventBus.Abstractions;
using Microsoft.Extensions.Options;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBookingRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMQConfig>(configuration.GetSection("RabbitMQ"));
            services.AddSingleton<BuildingBlocks.RabbitMQ.Connections.IRabbitMQConnection, BuildingBlocks.RabbitMQ.Connections.RabbitMQConnection>();
            services.AddSingleton<IEventBus, RabbitMQEventBus>();
            
            return services;
        }

        public static async Task<IApplicationBuilder> SubscribeToEvents(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
            
            // Subscribe to events here if needed (e.g., ExperienceUpdatedEvent to update cached data)
            
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
