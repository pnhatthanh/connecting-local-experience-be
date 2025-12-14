using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Application.Interfaces;
using Experience.Application.Dtos;
using Experience.Application.Interfaces;
using Experience.Domain.Repositories;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace Experience.Application.Handlers.Queries.GetRecommendations
{
    public class GetRecommendationsQueryHandler : IQueryHandler<GetRecommendationsQuery, List<ExperienceRecommendationDto>>
    {
        private readonly IAIRecommendationService _aiRecommendationService;
        private readonly IExperienceRepository _experienceRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetRecommendationsQueryHandler> _logger;

        public GetRecommendationsQueryHandler(
            IAIRecommendationService aiRecommendationService,
            IExperienceRepository experienceRepository,
            IMapper mapper,
            ICurrentUserService currentUserService,
            ILogger<GetRecommendationsQueryHandler> logger)
        {
            _aiRecommendationService = aiRecommendationService;
            _experienceRepository = experienceRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<List<ExperienceRecommendationDto>> Handle(
            GetRecommendationsQuery request, 
            CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var aiResponse = await _aiRecommendationService.GetRecommendationsAsync(
                userId,
                request.TopK, 
                cancellationToken);

            if (aiResponse == null || !aiResponse.Recommendations.Any())
            {
                _logger.LogWarning("No recommendations returned from AI Service for user {UserId}", userId);
                return new List<ExperienceRecommendationDto>();
            }

            var experienceIds = new List<Guid>();
            foreach (var item in aiResponse.Recommendations)
            {
                if (Guid.TryParse(item.ExperienceId, out var id))
                {
                    experienceIds.Add(id);
                }
                else
                {
                    _logger.LogWarning("Invalid experience ID from AI: {ExperienceId}", item.ExperienceId);
                }
            }

            if (!experienceIds.Any())
            {
                _logger.LogWarning("No valid experience IDs found in AI response");
                return new List<ExperienceRecommendationDto>();
            }

            var experiences = await _experienceRepository.GetExperiencesByIdsAsync(experienceIds);
            var experienceDtos = _mapper.Map<List<ExperienceSummaryDto>>(experiences);

            var experienceMap = experienceDtos.ToDictionary(e => e.Id);
            var scoreMap = aiResponse.Recommendations.ToDictionary(
                r => Guid.Parse(r.ExperienceId),
                r => new { r.Score, r.Reason }
            );

            var result = new List<ExperienceRecommendationDto>();
            
            foreach (var expId in experienceIds)
            {
                if (experienceMap.TryGetValue(expId, out var experience) && 
                    scoreMap.TryGetValue(expId, out var scoreInfo))
                {
                    result.Add(new ExperienceRecommendationDto
                    {
                        Experience = experience,
                        Score = scoreInfo.Score,
                        Reason = scoreInfo.Reason
                    });
                }
            }

            _logger.LogInformation(
                "Successfully enriched {Count} recommendations for user {UserId}", 
                result.Count, 
                userId);

            return result;
        }
    }
}
