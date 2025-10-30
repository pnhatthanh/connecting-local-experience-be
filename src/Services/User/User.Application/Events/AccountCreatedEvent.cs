using BuildingBlocks.Application.EventBus.Events;

namespace User.Application.Events
{
    public class AccountCreatedEvent : IntegrationEvent
    {
        public Guid AccountId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public AccountCreatedEvent()
        {
        }

        public AccountCreatedEvent(Guid accountId, string fullName, string email, DateTime createdAt)
        {
            AccountId = accountId;
            FullName = fullName;
            Email = email;
            CreatedAt = createdAt;
        }
    }
}
