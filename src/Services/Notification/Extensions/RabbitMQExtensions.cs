using BuildingBlocks.Application.EventBus.Abstractions;
using BuildingBlocks.RabbitMQ;
using BuildingBlocks.RabbitMQ.Configurations;
using Notification.Subscriptions;

namespace Notification.Extensions;

public static class RabbitMQExtensions
{
    public static IServiceCollection AddNotificationRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMQConfig = new RabbitMQConfig
        {
            HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
            Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = configuration["RabbitMQ:UserName"] ?? "guest",
            Password = configuration["RabbitMQ:Password"] ?? "guest",
            VirtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/",
            ExchangeName = configuration["RabbitMQ:ExchangeName"] ?? "connecting_events",
            QueueName = configuration["RabbitMQ:QueueName"] ?? "notification_service_queue"
        };

        services.AddRabbitMQ(rabbitMQConfig);

        return services;
    }

    public static async Task SubscribeToEvents(this WebApplication app)
    {
        var eventBus = app.Services.GetRequiredService<IEventBus>();
        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        await EventBusSubscription.SubscribeAllEvents(eventBus, logger);
    }
}
