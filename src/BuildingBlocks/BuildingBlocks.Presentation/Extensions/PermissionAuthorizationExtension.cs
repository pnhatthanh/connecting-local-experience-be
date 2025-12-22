using BuildingBlocks.Presentation.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Presentation.Extensions
{
    public static class PermissionAuthorizationExtension
    {
        /// <summary>
        /// Thêm Permission-based Authorization vào DI container
        /// </summary>
        public static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            return services;
        }
    }
}
