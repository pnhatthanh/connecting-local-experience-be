using Experience.Application.Dtos;
using Experience.Domain.Repositories;
using Experience.Domain.Specifications;
using MapsterMapper;
using MediatR;

namespace Experience.Application.Handlers.Queries.GetExperiencesByHost
{
    public class GetExperiencesByHostQueryHandler : IRequestHandler<GetExperiencesByHostQuery, List<ExperienceDto>>
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly IMapper _mapper;

        public GetExperiencesByHostQueryHandler(
            IExperienceRepository experienceRepository,
            IMapper mapper)
        {
            _experienceRepository = experienceRepository;
            _mapper = mapper;
        }

        public async Task<List<ExperienceDto>> Handle(GetExperiencesByHostQuery request, CancellationToken cancellationToken)
        {
            var spec = new ExperienceByHostSpecification(request.HostId);
            var experiences = await _experienceRepository.GetAllAsync(spec, 
                e => e.Category, 
                e => e.Media, 
                e => e.Schedule);
            return _mapper.Map<List<ExperienceDto>>(experiences);
        }
    }
}
