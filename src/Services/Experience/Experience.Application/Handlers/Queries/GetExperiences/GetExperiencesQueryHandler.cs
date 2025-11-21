using BuildingBlocks.Application.Dtos;
using Experience.Application.Dtos;
using Experience.Domain.Repositories;
using Experience.Domain.Specifications;
using MapsterMapper;
using MediatR;

namespace Experience.Application.Handlers.Queries.GetExperiences
{
    public class GetExperiencesQueryHandler : IRequestHandler<GetExperiencesQuery, PaginationResult<ExperienceSummaryDto>>
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly IMapper _mapper;

        public GetExperiencesQueryHandler(IExperienceRepository experienceRepository, IMapper mapper)
        {
            _experienceRepository = experienceRepository;
            _mapper = mapper;
        }
    
        public async Task<PaginationResult<ExperienceSummaryDto>> Handle(GetExperiencesQuery request, CancellationToken cancellationToken)
        {
            var spec = new ExperienceFilterSpecification(
                SearchTerm: request.SearchTerm,
                HostId: request.HostId,
                CategoryIds: request.CategoryIds,
                MinDuration: request.MinDuration,
                MaxDuration: request.MaxDuration,
                MinPrice: request.MinPrice,
                MaxPrice: request.MaxPrice,
                Languages: request.Languages
            );
            
            var experiences = await _experienceRepository.GetPagedListAsync(
                specification: spec, 
                pageNumber: request.PageNumber, 
                pageSize: request.PageSize, 
                sortBy: request.SortBy, 
                isAscending: request.IsAscending,
                includes: [e => e.Category, e => e.Media]
            );
            var totalCount = await _experienceRepository.CountAsync(spec);
                
            return new PaginationResult<ExperienceSummaryDto>
            {
                Data = _mapper.Map<List<ExperienceSummaryDto>>(experiences),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
