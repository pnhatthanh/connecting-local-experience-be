using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Application.Behaviors;
using IAM.Application.Handlers.Commands.Register;
using BuildingBlocks.Application.EventBus.Abstractions;
using IAM.Application.Events;
using IAM.Application.EventHandlers;

namespace IAM.Application.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddIAMApplication(this IServiceCollection services)
        {
            services.AddMediatR(typeof(RegisterCommand).Assembly);
            services.AddValidatorsFromAssembly(typeof(RegisterCommand).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddScoped<IIntegrationEventHandler<HostProfileVerifiedEvent>, HostProfileVerifiedEventHandler>();
            return services;
        }
        public static async Task<IServiceProvider> SubscribeToEventsAsync(this IServiceProvider serviceProvider)
        {
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();
            await eventBus.SubscribeAsync<HostProfileVerifiedEvent, IIntegrationEventHandler<HostProfileVerifiedEvent>>();
            return serviceProvider;
        }
    }
}