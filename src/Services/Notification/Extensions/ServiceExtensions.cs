using Notification.Configurations;
using Notification.Handlers;
using Notification.Services;

namespace Notification.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddNotificationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure Settings
        services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
        services.Configure<EmailSettings>(configuration.GetSection("Email"));

        // Register Services
        services.AddSingleton<ITemplateService, TemplateService>();
        services.AddSingleton<IEmailService, EmailService>();

        // Register Event Handlers
        services.AddTransient<EmailConfirmationRequestedEventHandler>();
        services.AddTransient<PasswordResetRequestedEventHandler>();

        // Add Health Checks
        services.AddHealthChecks();

        return services;
    }
}
