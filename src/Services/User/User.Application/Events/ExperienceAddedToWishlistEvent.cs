using BuildingBlocks.Application.EventBus.Events;

namespace User.Application.Events;

public class ExperienceAddedToWishlistEvent : IntegrationEvent
{
    public Guid UserId { get; set; }
    public Guid ExperienceId { get; set; }

    public ExperienceAddedToWishlistEvent()
    {
    }

    public ExperienceAddedToWishlistEvent(
        Guid userId,
        Guid experienceId)
    {
        UserId = userId;
        ExperienceId = experienceId;
    }
}