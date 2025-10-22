using BuildingBlocks.Application.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Experience.Application.Handlers.Commands.CreateExperience;

namespace Experience.Application.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddExperienceApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateExperienceCommand).Assembly));
            services.AddValidatorsFromAssembly(typeof(CreateExperienceCommand).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
        }
    }
}
