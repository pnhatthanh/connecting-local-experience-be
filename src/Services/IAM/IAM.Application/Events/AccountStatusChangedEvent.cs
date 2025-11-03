using BuildingBlocks.Application.EventBus.Events;

namespace IAM.Application.Events
{
    public class AccountStatusChangedEvent : IntegrationEvent
    {
        public Guid AccountId { get; set; }
        public bool IsActive { get; set; }

        public AccountStatusChangedEvent(Guid accountId, bool isActive)
        {
            AccountId = accountId;
            IsActive = isActive;
        }
    }
}
