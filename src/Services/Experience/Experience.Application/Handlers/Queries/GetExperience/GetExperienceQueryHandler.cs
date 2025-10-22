using Experience.Application.Dtos;
using Experience.Domain.Repositories;
using MediatR;

namespace Experience.Application.Handlers.Queries.GetExperience
{
    public class GetExperienceQueryHandler : IRequestHandler<GetExperienceQuery, ExperienceDto?>
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly IExperienceMediaRepository _mediaRepository;
        private readonly IExperienceItineraryRepository _itineraryRepository;

        public GetExperienceQueryHandler(
            IExperienceRepository experienceRepository,
            IExperienceMediaRepository mediaRepository,
            IExperienceItineraryRepository itineraryRepository)
        {
            _experienceRepository = experienceRepository;
            _mediaRepository = mediaRepository;
            _itineraryRepository = itineraryRepository;
        }

        public async Task<ExperienceDto?> Handle(GetExperienceQuery request, CancellationToken cancellationToken)
        {
            var experience = await _experienceRepository.GetByIdAsync(request.ExperienceId);
            if (experience == null)
                return null;

            var media = await _mediaRepository.GetByExperienceIdAsync(experience.Id);
            var itineraries = await _itineraryRepository.GetByExperienceIdAsync(experience.Id);

            return new ExperienceDto
            {
                Id = experience.Id,
                HostId = experience.HostId,
                Title = experience.Title,
                Description = experience.Description,
                Location = new LocationDto
                {
                    Latitude = experience.Location.Y,
                    Longitude = experience.Location.X
                },
                Price = experience.Price,
                Duration = experience.Duration,
                MaxParticipants = experience.MaxParticipants,
                Category = experience.Category.ToString(),
                Amenities = experience.Amenities,
                ActivityLevel = experience.ActivityLevel.ToString(),
                SkillLevel = experience.SkillLevel.ToString(),
                MinAge = experience.MinAge,
                Accessibility = experience.Accessibility,
                Status = experience.Status.ToString(),
                CancellationPolicy = experience.CancellationPolicy,
                MeetingPoint = experience.MeetingPoint,
                Language = experience.Language,
                CreatedAt = experience.CreatedAt,
                UpdatedAt = experience.UpdatedAt,
                Media = media.Select(m => new ExperienceMediaDto
                {
                    Id = m.Id,
                    Url = m.Url,
                    Order = m.Order
                }).ToList(),
                Itineraries = itineraries.Select(i => new ExperienceItineraryDto
                {
                    Id = i.Id,
                    StepNumber = i.StepNumber,
                    PhotoUrl = i.PhotoUrl,
                    Title = i.Title,
                    Description = i.Description,
                    Location = i.Location != null ? new LocationDto
                    {
                        Latitude = i.Location.Y,
                        Longitude = i.Location.X
                    } : null
                }).ToList()
            };
        }
    }
}
