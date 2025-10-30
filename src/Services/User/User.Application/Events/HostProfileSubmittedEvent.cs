using BuildingBlocks.Application.EventBus.Events;

namespace User.Application.Events
{
    public class HostProfileSubmittedEvent : IntegrationEvent
    {
        public Guid HostProfileId { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? DocumentUrl { get; set; }

        public HostProfileSubmittedEvent()
        {
        }

        public HostProfileSubmittedEvent(Guid hostProfileId, Guid userId, string email, string fullName, string? documentUrl)
        {
            HostProfileId = hostProfileId;
            UserId = userId;
            Email = email;
            FullName = fullName;
            DocumentUrl = documentUrl;
        }
    }
}
