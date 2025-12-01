using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.EntityFramework;
using Booking.Application.Interfaces;
using Booking.Domain.Repositories;
using Booking.Infrastructure.Configurations;
using Booking.Infrastructure.Data;
using Booking.Infrastructure.Repositories;
using Booking.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBookingInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string 'DefaultConnection' not found.");
            
            services.AddDbContext<BookingDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
            
            services.AddUnitOfWork<BookingDbContext>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IRefundRepository, RefundRepository>();
            services.AddScoped<IBookingCancellationRepository, BookingCancellationRepository>();
            
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            
            // Configure Momo payment service
            services.Configure<MomoSettings>(options =>
            {
                configuration.GetSection(MomoSettings.MomoSettingsKey).Bind(options);
            });
            services.AddHttpClient<IMomoService, MomoService>();
            
            services.AddHttpClient<IExperienceService, ExperienceService>(client =>
            {
                var experienceApiUrl = configuration["Services:ExperienceApi"]
                    ?? throw new ArgumentNullException("Experience API URL not configured");
                client.BaseAddress = new Uri(experienceApiUrl);
            });

            services.AddHttpClient<IUserService, UserService>(client =>
            {
                var userApiUrl = configuration["Services:UserApi"]
                    ?? throw new ArgumentNullException("User API URL not configured");
                client.BaseAddress = new Uri(userApiUrl);
            });

            return services;
        }
    }
}
