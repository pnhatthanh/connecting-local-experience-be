using BuildingBlocks.Application.Behaviors;
using Experience.Application.EventHandlers;
using Experience.Application.Events;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Experience.Application.Handlers.Commands.CreateExperience;
using System.Reflection;
using BuildingBlocks.Application.EventBus.Abstractions;

namespace Experience.Application.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddExperienceApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateExperienceCommand).Assembly));
            services.AddValidatorsFromAssembly(typeof(CreateExperienceCommand).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Assembly.GetExecutingAssembly());
            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();
            
            // Register event handlers
            services.AddScoped<IIntegrationEventHandler<BookingConfirmedEvent>, BookingConfirmedEventHandler>();
            
            return services;
        }
        public static async Task<IServiceProvider> SubscribeToEventsAsync(this IServiceProvider serviceProvider)
        {
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();
            await eventBus.SubscribeAsync<BookingConfirmedEvent, BookingConfirmedEventHandler>();
            return serviceProvider;
        }
    }
}
