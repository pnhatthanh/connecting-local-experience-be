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
using Microsoft.Extensions.Options;

namespace Experience.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddExperienceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string 'DefaultConnection' not found.");

            // Configure PostgreSQL with NetTopologySuite for geography support
            services.AddDbContext<ExperienceDbContext>(options =>
            {
                options.UseNpgsql(connectionString, x => x.UseNetTopologySuite());
            });

            services.AddUnitOfWork<ExperienceDbContext>();

            // Register repositories
            services.AddScoped<IExperienceRepository, ExperienceRepository>();
            services.AddScoped<IExperienceScheduleRepository, ExperienceScheduleRepository>();
            services.AddScoped<IExperienceScheduleSlotRepository, ExperienceScheduleSlotRepository>();
            services.AddScoped<IExperienceMediaRepository, ExperienceMediaRepository>();
            services.AddScoped<IExperienceItineraryRepository, ExperienceItineraryRepository>();

            // Configure Cloudinary
            services.Configure<CloudinarySettings>(opts =>
            {
                var section = configuration.GetSection(CloudinarySettings.CloudinarySettingsKey);
                section.Bind(opts);
            });
            services.AddScoped<IPhotoService, CloudinaryService>();

            return services;
        }
    }
}
