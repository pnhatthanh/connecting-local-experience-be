using BuildingBlocks.Application.EventBus.Abstractions;
using Notification.Events;
using Notification.Handlers;

namespace Notification.Subscriptions;

public static class EventBusSubscription
{
    public static async Task SubscribeAllEvents(IEventBus eventBus, ILogger logger)
    {
        try
        {
            // Subscribe to Email Confirmation Event
            await eventBus.SubscribeAsync<EmailConfirmationRequestedEvent, EmailConfirmationRequestedEventHandler>();
            logger.LogInformation("Subscribed to {EventName}", nameof(EmailConfirmationRequestedEvent));

            // Subscribe to Password Reset Event
            await eventBus.SubscribeAsync<PasswordResetRequestedEvent, PasswordResetRequestedEventHandler>();
            logger.LogInformation("Subscribed to {EventName}", nameof(PasswordResetRequestedEvent));

            logger.LogInformation("All notification events subscribed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to subscribe to notification events");
            throw;
        }
    }
}
