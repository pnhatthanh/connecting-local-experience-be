using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using IAM.Application.Handlers.Commands.RegisterCommand;

namespace IAM.Application.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddIAMApplication(this IServiceCollection services)
        {
            // Add MediatR
            services.AddMediatR(typeof(RegisterCommand).Assembly);

            // Add FluentValidation
            services.AddValidatorsFromAssembly(typeof(RegisterCommand).Assembly);

            // TODO: Add MediatR behaviors when ValidationBehavior is available
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}