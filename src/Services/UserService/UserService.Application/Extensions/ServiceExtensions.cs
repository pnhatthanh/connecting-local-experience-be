using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Queries.GetUserById;

namespace UserService.Application.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddUserApplication(this IServiceCollection services)
        {
            // Add MediatR for CQRS
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetUserByIdQuery).Assembly));

            // Add FluentValidation
            services.AddValidatorsFromAssembly(typeof(GetUserByIdQuery).Assembly);

            return services;
        }
    }
}
