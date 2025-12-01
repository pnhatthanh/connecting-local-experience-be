using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.EntityFramework;
using Experience.Application.Interfaces;
using Experience.Domain.Repositories;
using Experience.Infrastructure.Configurations;
using Experience.Infrastructure.Data;
using Experience.Infrastructure.Repositories;
using Experience.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Experience.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddExperienceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string 'DefaultConnection' not found.");
            
            services.AddDbContext<ExperienceDbContext>(options =>
            {
                options.UseNpgsql(connectionString, x => x.UseNetTopologySuite());
            });
            
            services.AddUnitOfWork<ExperienceDbContext>();
            services.AddScoped<IExperienceRepository, ExperienceRepository>();
            services.AddScoped<IExperienceCategoryRepository, ExperienceCategoryRepository>();
            services.AddScoped<IExperienceScheduleRepository, ExperienceScheduleRepository>();
            services.AddScoped<IExperienceScheduleSlotRepository, ExperienceScheduleSlotRepository>();
            services.AddScoped<IExperienceMediaRepository, ExperienceMediaRepository>();
            services.AddScoped<IExperienceItineraryRepository, ExperienceItineraryRepository>();
            
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            
            services.Configure<CloudinarySettings>(options =>
            {
                configuration.GetSection(CloudinarySettings.CloudinarySettingsKey).Bind(options);
            });
            services.AddScoped<IPhotoService, CloudinaryService>();

            services.AddHttpClient();
            services.AddScoped<IUserServiceClient, UserServiceClient>();

            return services;
        }
    }
}
