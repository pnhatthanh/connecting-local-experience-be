using BuildingBlocks.Application.EventBus.Events;

namespace IAM.Application.Events
{
    public class HostProfileVerifiedEvent : IntegrationEvent
    {
        public Guid UserId { get; set; }
        public bool IsApproved { get; set; }
        public string? Reason { get; set; }

        public HostProfileVerifiedEvent()
        {
        }
        public HostProfileVerifiedEvent( Guid userId, bool isApproved, string? reason)
        {
            UserId = userId;
            IsApproved = isApproved;
            Reason = reason;
        }
    }
}
