using Microsoft.OpenApi.Models;

namespace APIGateway.Extensions;

public static class SwaggerExtension
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "API Gateway - All Services",
                Version = "v1",
                Description = "API Gateway for Connecting Experience - Aggregated Swagger Documentation"
            });

            // Add JWT Bearer authentication
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    public static WebApplication UseSwaggerConfiguration(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway v1");
            
            // Add endpoints for each microservice
            options.SwaggerEndpoint("/swagger-iam/swagger.json", "IAM Service");
            options.SwaggerEndpoint("/swagger-experience/swagger.json", "Experience Service");
            options.SwaggerEndpoint("/swagger-user/swagger.json", "User Service");
            options.SwaggerEndpoint("/swagger-booking/swagger.json", "Booking Service");

            options.RoutePrefix = "swagger";
            options.DocumentTitle = "Connecting Experience - API Documentation";
            options.DefaultModelsExpandDepth(-1); // Disable schema section
        });

        return app;
    }
}
