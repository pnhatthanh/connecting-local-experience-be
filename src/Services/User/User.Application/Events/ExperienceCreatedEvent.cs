using BuildingBlocks.Application.EventBus.Events;

namespace User.Application.Events
{
    public class ExperienceCreatedEvent : IntegrationEvent
    {
        public string ExperienceId { get; set; } = string.Empty;
        public Guid HostId { get; set; }
    }

}
