using BuildingBlocks.Application.Behaviors;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Booking.Application.Handlers.Commands.CreateBooking;
using System.Reflection;

namespace Booking.Application.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBookingApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateBookingCommand).Assembly));
            services.AddValidatorsFromAssembly(typeof(CreateBookingCommand).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Assembly.GetExecutingAssembly());
            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();
            
            return services;
        }
    }
}
