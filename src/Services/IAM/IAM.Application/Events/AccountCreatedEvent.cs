using BuildingBlocks.Application.EventBus.Events;

namespace IAM.Application.Events
{
    public class AccountCreatedEvent : IntegrationEvent
    {
        public int AccountId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public AccountCreatedEvent(int accountId, string fullName, string email, DateTime createdAt)
        {
            AccountId = accountId;
            FullName = fullName;
            Email = email;
            CreatedAt = createdAt;
        }
    }
}