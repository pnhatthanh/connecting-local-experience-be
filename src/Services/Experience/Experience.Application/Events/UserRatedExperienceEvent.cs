using BuildingBlocks.Application.EventBus.Events;

namespace Experience.Application.Events;

public class UserRatedExperienceEvent : IntegrationEvent
{
    public Guid UserId { get; set; } = Guid.Empty;
    public Guid ExperienceId { get; set; } = Guid.Empty;
    public int Rating { get; set; }
    public UserRatedExperienceEvent()
    {
    }

    public UserRatedExperienceEvent(
        Guid userId,
        Guid experienceId,
        int rating)
    {
        UserId = userId;
        ExperienceId = experienceId;
        Rating = rating;
    }
}
