using System.Linq.Expressions;
using BuildingBlocks.Domain.Exceptions;
using Experience.Application.Dtos;
using Experience.Domain.Entities;
using Experience.Domain.Repositories;
using Experience.Domain.Specifications;
using MapsterMapper;
using MediatR;

namespace Experience.Application.Handlers.Queries.GetExperience
{
    public class GetExperienceQueryHandler : IRequestHandler<GetExperienceQuery, ExperienceDto?>
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly IMapper _mapper;

        public GetExperienceQueryHandler(
            IExperienceRepository experienceRepository,
            IMapper mapper)
        {
            _experienceRepository = experienceRepository;
            _mapper = mapper;
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
            var experience = await _experienceRepository.GetAnyAsync(spec, includes)
                ?? throw new BadRequestException($"Experience with ID {request.ExperienceId} not found.");
            return _mapper.Map<ExperienceDto>(experience);
        }
    }
}
