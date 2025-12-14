using System.Linq.Expressions;
using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Application.EventBus.Abstractions;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using Experience.Application.Dtos;
using Experience.Application.Events;
using Experience.Application.Interfaces;
using Experience.Domain.Entities;
using Experience.Domain.Repositories;
using Experience.Domain.Specifications;
using MapsterMapper;

namespace Experience.Application.Handlers.Queries.GetExperience
{
    public class GetExperienceQueryHandler : IQueryHandler<GetExperienceQuery, ExperienceDto?>
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly IMapper _mapper;
        private readonly IUserServiceClient _userServiceClient;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEventBus _eventBus;

        public GetExperienceQueryHandler(
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

        public async Task<ExperienceDto?> Handle(GetExperienceQuery request, CancellationToken cancellationToken)
        {
            var spec = new ExperienceIdSpecification(request.ExperienceId);
            var includes = new Expression<Func<ExperienceEntity, object>>[]
            {
                e => e.Category,
                e => e.Media,
                e => e.Schedule,
                e => e.Itineraries
            };
            var experience = await _experienceRepository.GetBySpecAsync(spec, includes)
                ?? throw new BadRequestException($"Experience with ID {request.ExperienceId} not found.");
            
            var experienceDto = _mapper.Map<ExperienceDto>(experience);

            // Check if experience is in user's wishlist
            if (_currentUserService.IsAuthenticated)
            {
                var userId = _currentUserService.UserId;
                var favoriteIds = await _userServiceClient.CheckExperiencesInWishlistAsync(userId, new List<Guid> { experienceDto.Id });
                experienceDto.IsFavorite = favoriteIds.Contains(experienceDto.Id);
                // Publish UserViewedEvent
                var userViewedEvent = new UserViewedEvent(
                    userId: userId,
                    experienceId: experienceDto.Id
                );
                await _eventBus.PublishAsync(userViewedEvent);
            }

            return experienceDto;
        }
    }
}
