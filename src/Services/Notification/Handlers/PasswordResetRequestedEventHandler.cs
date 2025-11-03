using BuildingBlocks.Application.EventBus.Abstractions;
using Microsoft.Extensions.Options;
using Notification.Configurations;
using Notification.Events;
using Notification.Services;

namespace Notification.Handlers;

public class PasswordResetRequestedEventHandler : IIntegrationEventHandler<PasswordResetRequestedEvent>
{
    private readonly IEmailService _emailService;
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<PasswordResetRequestedEventHandler> _logger;

    public PasswordResetRequestedEventHandler(
        IEmailService emailService,
        IOptions<EmailSettings> emailSettings,
        ILogger<PasswordResetRequestedEventHandler> logger)
    {
        _emailService = emailService;
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task HandleAsync(PasswordResetRequestedEvent @event, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Handling password reset request for user {Email} with token {Token}", 
                @event.Email, 
                @event.ResetToken);

            await _emailService.SendPasswordResetAsync(
                @event.Email,
                @event.FullName,
                @event.ResetToken);

            _logger.LogInformation(
                "Password reset email sent successfully to {Email}", 
                @event.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Failed to send password reset email to {Email}", 
                @event.Email);
        }
    }
}
