using BuildingBlocks.Application.CQRS.Query;
using Experience.Application.Dtos;
using Experience.Application.Interfaces;
using Experience.Domain.Repositories;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace Experience.Application.Handlers.Queries.GetPopularExperience
{
    public class GetPopularExperienceQueryHandler : IQueryHandler<GetPopularExperienceQuery, List<ExperienceRecommendationDto>>
    {
        private readonly IAIRecommendationService _aiRecommendationService;
        private readonly IExperienceRepository _experienceRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPopularExperienceQueryHandler> _logger;

        public GetPopularExperienceQueryHandler(
            IAIRecommendationService aiRecommendationService,
            IExperienceRepository experienceRepository,
            IMapper mapper,
            ILogger<GetPopularExperienceQueryHandler> logger)
        {
            _aiRecommendationService = aiRecommendationService;
            _experienceRepository = experienceRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ExperienceRecommendationDto>> Handle( GetPopularExperienceQuery request, CancellationToken cancellationToken)
        {
            var aiResponse = await _aiRecommendationService.GetPopularExperiencesAsync(request.TopK);
            if (aiResponse == null || !aiResponse.Recommendations.Any())
            {
                _logger.LogWarning("No popular experiences returned from AI Service");
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

            return result;
        }
    }
}
