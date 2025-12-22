using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace APIGateway.Extensions;

public static class HealthCheckExtension
{
    public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services)
    {
        var healthChecks = services.AddHealthChecks();
        healthChecks.AddCheck("self", () => HealthCheckResult.Healthy("API Gateway is healthy"));
        return services;
    }
}