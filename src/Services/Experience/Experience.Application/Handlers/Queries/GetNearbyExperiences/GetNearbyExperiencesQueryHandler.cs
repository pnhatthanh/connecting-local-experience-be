using BuildingBlocks.Application.CQRS.Query;
using Experience.Application.Dtos;
using Experience.Domain.Repositories;
using NetTopologySuite.Geometries;

namespace Experience.Application.Handlers.Queries.GetNearbyExperiences
{
    public class GetNearbyExperiencesQueryHandler : IQueryHandler<GetNearbyExperiencesQuery, IEnumerable<ExperienceDto>>
    {
        private readonly IExperienceRepository _experienceRepository;

        public GetNearbyExperiencesQueryHandler(IExperienceRepository experienceRepository)
        {
            _experienceRepository = experienceRepository;
        }

        public async Task<IEnumerable<ExperienceDto>> Handle(GetNearbyExperiencesQuery request, CancellationToken cancellationToken)
        {
            var userLocation = new Point(request.Longitude, request.Latitude) { SRID = 4326 };

            var radiusInMeters = request.RadiusInKm * 1000;

            var experiences = await _experienceRepository.GetNearbyExperiencesAsync(userLocation, radiusInMeters);

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
                ActivityLevel = e.ActivityLevel.ToString(),
                SkillLevel = e.SkillLevel.ToString(),
                MinAge = e.MinAge,
                Accessibility = e.Accessibility,
                Amenities = e.Amenities,
                Status = e.Status.ToString(),
                CancellationPolicy = e.CancellationPolicy,
                MeetingPoint = e.MeetingPoint,
                Language = e.Language,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                Media = e.Media.Select(m => new ExperienceMediaDto
                {
                    Id = m.Id,
                    Url = m.Url,
                    Order = m.Order
                }).ToList(),
                Distance = Math.Round(e.Location.Distance(userLocation) / 1000, 2) 
            }).ToList();
        }
    }
}
