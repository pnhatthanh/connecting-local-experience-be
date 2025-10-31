using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Application.Behaviors;
using IAM.Application.Handlers.Commands.Register;

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