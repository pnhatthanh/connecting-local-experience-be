using BuildingBlocks.Presentation.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace BuildingBlocks.Presentation.Extensions
{
    public static class MiddlewareExtension
    {
        public static IApplicationBuilder UseException(this IApplicationBuilder application)
        {
            application.UseMiddleware<ExceptionMiddleware>();
            return application;
        }
        public static IApplicationBuilder UseSecurityHeader(this IApplicationBuilder application)
        {
            application.UseMiddleware<SecurityHeaderMiddleware>();
            return application;
        }        
    }
}
