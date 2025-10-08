using BuildingBlocks.Application.EventBus.Events;

namespace IAM.Application.Events
{
    public class EmailConfirmationRequestedEvent : IntegrationEvent
    {
        public Guid AccountId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ConfirmationToken { get; set; } = string.Empty;

        public EmailConfirmationRequestedEvent(Guid accountId, string fullName, string email, string confirmationToken)
        {
            AccountId = accountId;
            FullName = fullName;
            Email = email;
            ConfirmationToken = confirmationToken;
        }
    }
}
