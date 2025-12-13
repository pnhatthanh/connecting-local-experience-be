using BuildingBlocks.Application.Dtos;
using BuildingBlocks.Application.EventBus.Abstractions;
using BuildingBlocks.Application.Interfaces;
using Experience.Application.Dtos;
using Experience.Application.Events;
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
        private readonly IEventBus _eventBus;

        public GetExperiencesQueryHandler(
            IExperienceRepository experienceRepository, 
            IMapper mapper,
            IUserServiceClient userServiceClient,
            ICurrentUserService currentUserService,
            IEventBus eventBus)
        {
            _experienceRepository = experienceRepository;
            _mapper = mapper;
            _userServiceClient = userServiceClient;
            _currentUserService = currentUserService;
            _eventBus = eventBus;
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

            if (_currentUserService.IsAuthenticated && experienceDtos.Any())
            {
                var userId = _currentUserService.UserId;
                var experienceIds = experienceDtos.Select(e => e.Id).ToList();
                var favoriteIds = await _userServiceClient.CheckExperiencesInWishlistAsync(userId, experienceIds);
                foreach (var dto in experienceDtos)
                {
                    dto.IsFavorite = favoriteIds.Contains(dto.Id);
                }
                if(HasSearchCriteria(request))
                {
                    var userSearchedEvent = new UserSearchedEvent(
                        userId: _currentUserService.UserId,
                        experienceIds: experienceIds
                    );
                    await _eventBus.PublishAsync(userSearchedEvent);
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
        private bool HasSearchCriteria(GetExperiencesQuery request)
        {
            return request.SearchTerm is not null || request.CategoryIds is not null 
                || request.MinPrice is not null || request.MaxPrice is not null 
                || request.MinDuration is not null || request.MaxDuration is not null
                || request.Languages is not null || request.Status is not null;
        }
    }
}
