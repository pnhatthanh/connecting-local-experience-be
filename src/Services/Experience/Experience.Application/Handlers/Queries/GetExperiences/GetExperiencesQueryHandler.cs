using BuildingBlocks.Application.Dtos;
using Experience.Application.Dtos;
using Experience.Domain.Enums;
using Experience.Domain.Repositories;
using MediatR;

namespace Experience.Application.Handlers.Queries.GetExperiences
{
    public class GetExperiencesQueryHandler : IRequestHandler<GetExperiencesQuery, PaginationResult<ExperienceDto>>
    {
        private readonly IExperienceRepository _experienceRepository;

        public GetExperiencesQueryHandler(IExperienceRepository experienceRepository)
        {
            _experienceRepository = experienceRepository;
        }

        public async Task<PaginationResult<ExperienceDto>> Handle(GetExperiencesQuery request, CancellationToken cancellationToken)
        {
            var experiences = await _experienceRepository.GetAllAsync();

            var filteredExperiences = experiences.AsEnumerable();

            if (!string.IsNullOrEmpty(request.Category))
            {
                if (Enum.TryParse<ExperienceCategory>(request.Category, out var category))
                {
                    filteredExperiences = filteredExperiences.Where(e => e.Category == category);
                }
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                if (Enum.TryParse<ExperienceStatus>(request.Status, out var status))
                {
                    filteredExperiences = filteredExperiences.Where(e => e.Status == status);
                }
            }

            var totalCount = filteredExperiences.Count();
            var items = filteredExperiences
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(e => new ExperienceDto
                {
                    Id = e.Id,
                    HostId = e.HostId,
                    Title = e.Title,
                    Description = e.Description,
                    Location = new LocationDto
                    {
                        Latitude = e.Location.Y,
                        Longitude = e.Location.X
                    },
                    Price = e.Price,
                    Duration = e.Duration,
                    MaxParticipants = e.MaxParticipants,
                    Category = e.Category.ToString(),
                    Amenities = e.Amenities,
                    ActivityLevel = e.ActivityLevel.ToString(),
                    SkillLevel = e.SkillLevel.ToString(),
                    MinAge = e.MinAge,
                    Accessibility = e.Accessibility,
                    Status = e.Status.ToString(),
                    CancellationPolicy = e.CancellationPolicy,
                    MeetingPoint = e.MeetingPoint,
                    Language = e.Language,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    Media = e.Media?.Select(m => new ExperienceMediaDto
                    {
                        Id = m.Id,
                        Url = m.Url,
                        Order = m.Order
                    }).ToList()
                })
                .ToList();

            return new PaginationResult<ExperienceDto>
            {
                Data = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
