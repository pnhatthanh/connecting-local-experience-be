using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.EntityFramework;
using BuildingBlocks.RabbitMQ;
using BuildingBlocks.RabbitMQ.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using User.Application.Interfaces;
using User.Domain.Repositories;
using User.Infrastructure.Configurations;
using User.Infrastructure.Data;
using User.Infrastructure.Repositories;
using User.Infrastructure.Services;

namespace User.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddUserInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string 'DefaultConnection' not found.");
            services.AddDbContextPostgreSql<UserDbContext>(connectionString);
            services.AddUnitOfWork<UserDbContext>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IHostProfileRepository, HostProfileRepository>();
            services.AddScoped<IUserFavoriteExperienceRepository, UserFavoriteExperienceRepository>();

            var rabbitMQSetting = configuration.GetSection("RabbitMQ").Get<RabbitMQConfig>()
                ?? throw new ArgumentNullException("RabbitMQ configuration is null");
            services.AddRabbitMQ(rabbitMQSetting);

            services.Configure<CloudinarySettings>(configuration.GetSection(CloudinarySettings.CloudinarySettingsKey));
            services.AddScoped<IPhotoService, CloudinaryService>();
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }
}
