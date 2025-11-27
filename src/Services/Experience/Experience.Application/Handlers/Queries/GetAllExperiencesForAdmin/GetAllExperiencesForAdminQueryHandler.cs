using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Application.Dtos;
using Experience.Application.Dtos;
using Experience.Domain.Repositories;
using Experience.Domain.Specifications;
using MapsterMapper;

namespace Experience.Application.Handlers.Queries.GetAllExperiencesForAdmin
{
    public class GetAllExperiencesForAdminQueryHandler : IQueryHandler<GetAllExperiencesForAdminQuery, PaginationResult<ExperienceSummaryDto>>
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly IMapper _mapper;

        public GetAllExperiencesForAdminQueryHandler(IExperienceRepository experienceRepository, IMapper mapper)
        {
            _experienceRepository = experienceRepository;
            _mapper = mapper;
        }

        public async Task<PaginationResult<ExperienceSummaryDto>> Handle(GetAllExperiencesForAdminQuery request, CancellationToken cancellationToken)
        {
            var spec = new AdminExperienceFilterSpecification(
                searchTerm: request.SearchTerm,
                status: request.Status,
                categoryId: request.CategoryId
            );

            var experiences = await _experienceRepository.GetPagedListAsync(
                specification: spec,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                sortBy: request.SortBy ?? "CreatedAt",
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
