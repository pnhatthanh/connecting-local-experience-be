using BuildingBlocks.Application.EventBus.Events;

namespace Experience.Application.Events;

public class UserViewedEvent : IntegrationEvent
{
    public Guid UserId { get; set; } = Guid.Empty;
    public Guid ExperienceId { get; set; } = Guid.Empty;

    public UserViewedEvent()
    {
    }
    public UserViewedEvent(
        Guid userId,
        Guid experienceId)
    {
        UserId = userId;
        ExperienceId = experienceId;
    }
}