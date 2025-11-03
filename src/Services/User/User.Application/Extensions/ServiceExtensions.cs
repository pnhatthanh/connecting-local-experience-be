using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using BuildingBlocks.Application.Behaviors;
using User.Application.Handlers.Commands.UpdateUserProfile;
using BuildingBlocks.Application.EventBus.Abstractions;
using User.Application.Events;
using User.Application.EventHandlers;

namespace User.Application.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddUserApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UpdateUserProfileCommand).Assembly));
            services.AddValidatorsFromAssembly(typeof(UpdateUserProfileCommand).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Assembly.GetExecutingAssembly());
            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();

            services.AddScoped<IIntegrationEventHandler<AccountCreatedEvent>, AccountCreatedEventHandler>();
            services.AddScoped<IIntegrationEventHandler<AccountStatusChangedEvent>, AccountStatusChangedEventHandler>();
            return services;
        }
        
        public static async Task<IServiceProvider> SubscribeToEventsAsync(this IServiceProvider serviceProvider)
        {
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();
            await eventBus.SubscribeAsync<AccountCreatedEvent, IIntegrationEventHandler<AccountCreatedEvent>>();
            await eventBus.SubscribeAsync<AccountStatusChangedEvent, IIntegrationEventHandler<AccountStatusChangedEvent>>();
            return serviceProvider;
        }
    }
}
