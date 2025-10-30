using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Application.Behaviors;
using User.Application.Handlers.Commands.UpdateUserProfile;
using BuildingBlocks.Application.EventBus.Abstractions;
using User.Application.Events;

namespace User.Application.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddUserApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UpdateUserProfileCommand).Assembly));
            services.AddValidatorsFromAssembly(typeof(UpdateUserProfileCommand).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
        
        public static async Task<IServiceProvider> SubscribeToEventsAsync(this IServiceProvider serviceProvider)
        {
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();
            await eventBus.SubscribeAsync<AccountCreatedEvent, IIntegrationEventHandler<AccountCreatedEvent>>();
            return serviceProvider;
        }
    }
}
