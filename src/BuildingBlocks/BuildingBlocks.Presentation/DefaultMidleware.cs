using BuildingBlocks.Presentation.Extensions;
using Microsoft.AspNetCore.Builder;

namespace BuildingBlocks.Presentation
{
    public static class DefaultMiddleware
    {
        public static IApplicationBuilder UseDefaultMiddlewares(this IApplicationBuilder app)
        {
            app.UseException();
            app.UseSecurityHeader();
            return app;
        }
    }
}
