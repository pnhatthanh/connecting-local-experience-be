using BuildingBlocks.Application.EventBus.Events;

namespace Experience.Application.Events
{
    public class UserSearchedEvent : IntegrationEvent
    {        
        public Guid UserId { get; set; } = Guid.Empty;
        public List<Guid> ExperienceIds { get; set; } = new();

        public UserSearchedEvent()
        {
        }
        public UserSearchedEvent(
            Guid userId,
            List<Guid> experienceIds)
        {
            UserId = userId;
            ExperienceIds = experienceIds;
        }
    }
}
