using BuildingBlocks.EntityFramework;
using BuildingBlocks.RabbitMQ;
using BuildingBlocks.RabbitMQ.Configurations;
using IAM.Application.Interfaces;
using IAM.Domain.Repositories;
using IAM.Infrastructure.Configurations;
using IAM.Infrastructure.Data;
using IAM.Infrastructure.Repositories;
using IAM.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;



namespace IAM.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddIAMInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string 'DefaultConnection' not found.");
            services.AddDbContextPostgreSql<IAMDbContext>(connectionString);
            services.AddUnitOfWork<IAMDbContext>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.Configure<JwtSetting>(configuration.GetSection(JwtSetting.JwtSettingKey));
            var rabbitMQSetting = configuration.GetSection("RabbitMQ").Get<RabbitMQConfig>()
                ?? throw new ArgumentNullException("RabbitMQ configuration is null");
            services.AddRabbitMQ(rabbitMQSetting);
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            return services;
        }
    }
}