using Notification.Configurations;
using Notification.Handlers;
using Notification.Services;

namespace Notification.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddNotificationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
        services.Configure<EmailSettings>(configuration.GetSection("Email"));
        services.AddSingleton<ITemplateService, TemplateService>();
        services.AddSingleton<IEmailService, EmailService>();
        services.AddTransient<EmailConfirmationRequestedEventHandler>();
        services.AddTransient<PasswordResetRequestedEventHandler>();
        services.AddHealthChecks();

        return services;
    }
}
