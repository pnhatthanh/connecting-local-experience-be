using BuildingBlocks.Application.EventBus.Events;

namespace Experience.Application.Events;

public class UserRatedExperienceEvent : IntegrationEvent
{
    public Guid UserId { get; set; } = Guid.Empty;
    public Guid HostId { get; set; } = Guid.Empty;
    public Guid ExperienceId { get; set; } = Guid.Empty;
    public int Rating { get; set; }
    public UserRatedExperienceEvent()
    {
    }

    public UserRatedExperienceEvent(
        Guid userId,
        Guid hostId,
        Guid experienceId,
        int rating)
    {
        UserId = userId;
        HostId = hostId;
        ExperienceId = experienceId;
        Rating = rating;
    }
}
