using BuildingBlocks.Application.CQRS.Query;
using Experience.Application.Dtos;
using Experience.Domain.Repositories;
using Experience.Domain.Specifications;
using MapsterMapper;
using NetTopologySuite.Geometries;

namespace Experience.Application.Handlers.Queries.GetNearbyExperiences
{
    public class GetNearbyExperiencesQueryHandler : IQueryHandler<GetNearbyExperiencesQuery, IEnumerable<ExperienceDto>>
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly IMapper _mapper;

        public GetNearbyExperiencesQueryHandler(IExperienceRepository experienceRepository, IMapper mapper)
        {
            _experienceRepository = experienceRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ExperienceDto>> Handle(GetNearbyExperiencesQuery request, CancellationToken cancellationToken)
        {
            var userLocation = new Point(request.Longitude, request.Latitude) { SRID = 4326 };
            var radiusInMeters = request.RadiusInKm * 1000;

            var spec = new NearbyExperiencesSpecification(userLocation, radiusInMeters);
            var experiences = await _experienceRepository.GetAllAsync(spec, 
                e => e.Category, 
                e => e.Media, 
                e => e.Schedule);

            return experiences
                .OrderBy(e => e.Location.Distance(userLocation))
                .Select(e =>
                {
                    var dto = _mapper.Map<ExperienceDto>(e);
                    dto.Distance = Math.Round(e.Location.Distance(userLocation) / 1000, 2);
                    return dto;
                }).ToList();
        }
    }
}
