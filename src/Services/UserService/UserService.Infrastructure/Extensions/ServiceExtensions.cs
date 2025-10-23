using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.EntityFramework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using UserService.Domain.Repositories;
using UserService.Infrastructure.Data;
using UserService.Infrastructure.Repositories;
using UserService.Infrastructure.Services;

namespace UserService.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddUserInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string 'DefaultConnection' not found.");
            
            services.AddDbContextSqlServer<UserDbContext>(connectionString);
            services.AddUnitOfWork<UserDbContext>();

            // Repositories
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();

            // Services
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }
}
