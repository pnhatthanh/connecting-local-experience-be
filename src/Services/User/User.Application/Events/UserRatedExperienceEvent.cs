using BuildingBlocks.Application.EventBus.Events;

namespace User.Application.Events;

public class UserRatedExperienceEvent : IntegrationEvent
{
    public Guid UserId { get; set; } = Guid.Empty;
    public Guid HostId { get; set; } = Guid.Empty;
    public int Rating { get; set; }
}
