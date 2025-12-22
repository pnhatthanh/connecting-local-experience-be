using BuildingBlocks.Application.CQRS.Query;
using Experience.Application.Dtos;
using Experience.Domain.Repositories;
using Mapster;
using MapsterMapper;

namespace Experience.Application.Handlers.Queries.GetExperiencesByIds
{
    public class GetExperiencesByIdsQueryHandler : IQueryHandler<GetExperiencesByIdsQuery, List<ExperienceSummaryDto>>
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly IMapper _mapper;

        public GetExperiencesByIdsQueryHandler(IExperienceRepository experienceRepository, IMapper mapper)
        {
            _experienceRepository = experienceRepository;
            _mapper = mapper;
        }

        public async Task<List<ExperienceSummaryDto>> Handle(GetExperiencesByIdsQuery request, CancellationToken cancellationToken)
        {
            if (!request.Ids.Any())
                return new List<ExperienceSummaryDto>();

            var experiences = await _experienceRepository.GetExperiencesByIdsAsync(request.Ids);
            return _mapper.Map<List<ExperienceSummaryDto>>(experiences);
        }
    }
}
