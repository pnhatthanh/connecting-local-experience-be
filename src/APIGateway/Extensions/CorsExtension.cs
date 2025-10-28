namespace APIGateway.Extensions;

public static class CorsExtension
{
    public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration configuration)
    {
        var corsSettings = configuration.GetSection(Configurations.CorsSetting.SectionName).Get<Configurations.CorsSetting>()
            ?? throw new InvalidOperationException("Cors settings are not configured properly.");
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                // Check if wildcard is used
                if (corsSettings.AllowedOrigins.Contains("*"))
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                }
                else
                {
                    builder.WithOrigins(corsSettings.AllowedOrigins)
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                }
            });
        });
        return services;
    }
}