using BuildingBlocks.Application.EventBus.Events;

namespace Notification.Events;

public class PasswordResetRequestedEvent : IntegrationEvent
{
    public Guid AccountId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ResetToken { get; set; } = string.Empty;

    public PasswordResetRequestedEvent() { }

    public PasswordResetRequestedEvent(Guid accountId, string fullName, string email, string resetToken)
    {
        AccountId = accountId;
        FullName = fullName;
        Email = email;
        ResetToken = resetToken;
    }
}
