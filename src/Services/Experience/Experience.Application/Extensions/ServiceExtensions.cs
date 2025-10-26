using BuildingBlocks.Application.Behaviors;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Experience.Application.Handlers.Commands.CreateExperience;
using System.Reflection;

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
            
            return services;
        }
    }
}
