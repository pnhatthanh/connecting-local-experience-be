using BuildingBlocks.Application.EventBus.Abstractions;
using Microsoft.Extensions.Options;
using Notification.Configurations;
using Notification.Events;
using Notification.Services;

namespace Notification.Handlers;

public class EmailConfirmationRequestedEventHandler : IIntegrationEventHandler<EmailConfirmationRequestedEvent>
{
    private readonly IEmailService _emailService;
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailConfirmationRequestedEventHandler> _logger;

    public EmailConfirmationRequestedEventHandler(
        IEmailService emailService,
        IOptions<EmailSettings> emailSettings,
        ILogger<EmailConfirmationRequestedEventHandler> logger)
    {
        _emailService = emailService;
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task HandleAsync(EmailConfirmationRequestedEvent @event, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Handling email confirmation request for user {Email} with token {Token}", 
                @event.Email, 
                @event.ConfirmationToken);

            _logger.LogInformation(
                "VerificationUrlTemplate: {Template}", 
                _emailSettings.VerificationUrlTemplate);

            var confirmationUrl = _emailSettings.VerificationUrlTemplate
                .Replace("{email}", Uri.EscapeDataString(@event.Email))
                .Replace("{token}", @event.ConfirmationToken);

            _logger.LogInformation(
                "Generated confirmation URL: {ConfirmationUrl}", 
                confirmationUrl);

            await _emailService.SendEmailConfirmationAsync(
                @event.Email,
                @event.FullName,
                confirmationUrl);

            _logger.LogInformation(
                "Email confirmation sent successfully to {Email}", 
                @event.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Failed to send email confirmation to {Email}", 
                @event.Email);
        }
    }
}
