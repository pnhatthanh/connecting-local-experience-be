using BuildingBlocks.Application.EventBus.Events;
using Experience.Application.Dtos;

namespace Experience.Application.Events
{
    public class ExperienceCreatedEvent : IntegrationEvent
    {
        public string ExperienceId { get; set; } = string.Empty;
        public Guid HostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MaxParticipants { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal AdultPrice { get; set; }
        public decimal ChildPrice { get; set; }
        public int Duration { get; set; }
        public string Language { get; set; } = string.Empty;
        public List<ExperienceMediaDto> Media { get; set; } = new();

        public ExperienceCreatedEvent()
        {
        }

        public ExperienceCreatedEvent(
            string experienceId,
            Guid hostId,
            string title,
            string description,
            int maxParticipants,
            string address,
            string category,
            decimal adultPrice,
            decimal childPrice,
            int duration,
            string language,
            List<ExperienceMediaDto> media)
        {
            ExperienceId = experienceId;
            HostId = hostId;
            Title = title;
            Description = description;
            MaxParticipants = maxParticipants;
            Address = address;
            Category = category;
            AdultPrice = adultPrice;
            ChildPrice = childPrice;
            Duration = duration;
            Language = language;
            Media = media;
        }
    }
}
