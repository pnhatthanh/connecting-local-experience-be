using Experience.Application.Dtos;
using Experience.Domain.Repositories;
using MediatR;

namespace Experience.Application.Handlers.Queries.GetExperiencesByHost
{
    public class GetExperiencesByHostQueryHandler : IRequestHandler<GetExperiencesByHostQuery, List<ExperienceDto>>
    {
        private readonly IExperienceRepository _experienceRepository;

        public GetExperiencesByHostQueryHandler(IExperienceRepository experienceRepository)
        {
            _experienceRepository = experienceRepository;
        }

        public async Task<List<ExperienceDto>> Handle(GetExperiencesByHostQuery request, CancellationToken cancellationToken)
        {
            var experiences = await _experienceRepository.GetByHostIdAsync(request.HostId);

            return experiences.Select(e => new ExperienceDto
            {
                Id = e.Id,
                HostId = e.HostId,
                Title = e.Title,
                Description = e.Description,
                Location = new LocationDto
                {
                    Latitude = e.Location.Y,
                    Longitude = e.Location.X
                },
                Price = e.Price,
                Duration = e.Duration,
                MaxParticipants = e.MaxParticipants,
                Category = e.Category.ToString(),
                Amenities = e.Amenities,
                ActivityLevel = e.ActivityLevel.ToString(),
                SkillLevel = e.SkillLevel.ToString(),
                MinAge = e.MinAge,
                Accessibility = e.Accessibility,
                Status = e.Status.ToString(),
                CancellationPolicy = e.CancellationPolicy,
                MeetingPoint = e.MeetingPoint,
                Language = e.Language,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                Media = e.Media?.Select(m => new ExperienceMediaDto
                {
                    Id = m.Id,
                    Url = m.Url,
                    Order = m.Order
                }).ToList()
            }).ToList();
        }
    }
}
