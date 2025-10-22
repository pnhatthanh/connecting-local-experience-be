using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using IAM.Application.Handlers.Commands.RegisterCommand;
using BuildingBlocks.Application.Behaviors;

namespace IAM.Application.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddIAMApplication(this IServiceCollection services)
        {
            services.AddMediatR(typeof(RegisterCommand).Assembly);
            services.AddValidatorsFromAssembly(typeof(RegisterCommand).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
        }
    }
}