using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.EntityFramework;
using Booking.Application.Interfaces;
using Booking.Domain.Repositories;
using Booking.Infrastructure.Data;
using Booking.Infrastructure.Repositories;
using Booking.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VNPAY.Extensions;
using BuildingBlocks.RabbitMQ.Configurations;
using BuildingBlocks.RabbitMQ;

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

            var rabbitMQSetting = configuration.GetSection("RabbitMQ").Get<RabbitMQConfig>()
                ?? throw new ArgumentNullException("RabbitMQ configuration is null");
            services.AddRabbitMQ(rabbitMQSetting);
            
            // Configure VNPAY.NET library
            services.AddVnpayClient(config =>
            {
                config.TmnCode = configuration["VnPaySettings:TmnCode"]!;
                config.HashSecret = configuration["VnPaySettings:HashSecret"]!;
                config.BaseUrl = configuration["VnPaySettings:BaseUrl"]!;
                config.CallbackUrl = configuration["VnPaySettings:ReturnUrl"]!;
                config.Version = configuration["VnPaySettings:Version"] ?? "2.1.0";
                config.OrderType = "other";
            });
            
            services.AddScoped<IVnPayService, VnPayService>();
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
