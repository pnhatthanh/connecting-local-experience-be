using BuildingBlocks.Application.Dtos;
using BuildingBlocks.Application.Interfaces;
using Experience.Application.Dtos;
using Experience.Application.Interfaces;
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
        private readonly IUserServiceClient _userServiceClient;
        private readonly ICurrentUserService _currentUserService;

        public GetExperiencesQueryHandler(
            IExperienceRepository experienceRepository, 
            IMapper mapper,
            IUserServiceClient userServiceClient,
            ICurrentUserService currentUserService)
        {
            _experienceRepository = experienceRepository;
            _mapper = mapper;
            _userServiceClient = userServiceClient;
            _currentUserService = currentUserService;
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
                Languages: request.Languages,
                Status: request.Status
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
            
            var experienceDtos = _mapper.Map<List<ExperienceSummaryDto>>(experiences);

            // Check favorites for authenticated users
            if (_currentUserService.IsAuthenticated && experienceDtos.Any())
            {
                var userId = _currentUserService.UserId;
                var experienceIds = experienceDtos.Select(e => e.Id).ToList();
                var favoriteIds = await _userServiceClient.CheckExperiencesInWishlistAsync(userId, experienceIds);
                
                foreach (var dto in experienceDtos)
                {
                    dto.IsFavorite = favoriteIds.Contains(dto.Id);
                }
            }
                
            return new PaginationResult<ExperienceSummaryDto>
            {
                Data = experienceDtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
