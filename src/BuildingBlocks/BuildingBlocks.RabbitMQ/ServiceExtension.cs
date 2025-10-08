using BuildingBlocks.Application.EventBus.Abstractions;
using BuildingBlocks.RabbitMQ.Configurations;
using BuildingBlocks.RabbitMQ.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace BuildingBlocks.RabbitMQ
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, RabbitMQConfig rabbitSettings)
        {
            Console.WriteLine("HostName: " + rabbitSettings.HostName);
            Console.WriteLine("Port: " + rabbitSettings.Port);
            Console.WriteLine("UserName: " + rabbitSettings.UserName);
            Console.WriteLine("VirtualHost: " + rabbitSettings.VirtualHost);

            // Register RabbitMQConfig
            services.AddSingleton(Options.Create(rabbitSettings));

            services.AddSingleton<IConnectionFactory>(sp =>
            {   
                var factory = new ConnectionFactory()
                {
                    HostName = rabbitSettings.HostName,
                    Port = rabbitSettings.Port,
                    UserName = rabbitSettings.UserName,
                    Password = rabbitSettings.Password,
                    VirtualHost = rabbitSettings.VirtualHost,
                    RequestedConnectionTimeout = TimeSpan.FromSeconds(rabbitSettings.ConnectionTimeout),
                    RequestedHeartbeat = TimeSpan.FromSeconds(rabbitSettings.RequestedHeartbeat)
                };

                return factory;
            });

            services.AddSingleton<IRabbitMQConnection>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<RabbitMQConfig>>();
                var logger = sp.GetRequiredService<ILogger<RabbitMQConnection>>();
                var factory = sp.GetRequiredService<IConnectionFactory>();
                
                return new RabbitMQConnection(options, logger, factory);
            });

            services.AddSingleton<IEventBus, RabbitMQEventBus>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<RabbitMQConfig>>();
                var logger = sp.GetRequiredService<ILogger<RabbitMQEventBus>>();
                var connection = sp.GetRequiredService<IRabbitMQConnection>();

                return new RabbitMQEventBus(options, sp, logger, connection);
            });

            return services;
        }
    }
}
