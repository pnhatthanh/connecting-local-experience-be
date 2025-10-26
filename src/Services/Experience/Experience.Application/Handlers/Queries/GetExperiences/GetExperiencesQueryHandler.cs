using BuildingBlocks.Application.Dtos;
using Experience.Application.Dtos;
using Experience.Application.Extensions;
using Experience.Domain.Enums;
using Experience.Domain.Repositories;
using Experience.Domain.Specifications;
using MapsterMapper;
using MediatR;

namespace Experience.Application.Handlers.Queries.GetExperiences
{
    public class GetExperiencesQueryHandler : IRequestHandler<GetExperiencesQuery, PaginationResult<ExperienceDto>>
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly IMapper _mapper;

        public GetExperiencesQueryHandler(IExperienceRepository experienceRepository, IMapper mapper)
        {
            _experienceRepository = experienceRepository;
            _mapper = mapper;
        }

        public async Task<PaginationResult<ExperienceDto>> Handle(GetExperiencesQuery request, CancellationToken cancellationToken)
        {
            Guid? categoryId = null;
            ExperienceStatus? status = null;

            if (!string.IsNullOrEmpty(request.Category) && Guid.TryParse(request.Category, out var parsedCategoryId))
                categoryId = parsedCategoryId;
            if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<ExperienceStatus>(request.Status, out var parsedStatus))
                status = parsedStatus;
            var spec = new ExperienceFilterSpecification(categoryId, status);
            var experiences = await _experienceRepository.GetPagedListAsync(spec, request.PageNumber, request.PageSize, null, true,
                e => e.Category, 
                e => e.Media);
            return new PaginationResult<ExperienceDto>
            {
                Data = _mapper.Map<List<ExperienceDto>>(experiences),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
