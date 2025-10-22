using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using Experience.Application.Dtos;
using Experience.Domain.Repositories;
using MediatR;

namespace Experience.Application.Handlers.Commands.UpdateExperience
{
    public class UpdateExperienceCommandHandler : IRequestHandler<UpdateExperienceCommand, ExperienceDto>
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateExperienceCommandHandler(
            IExperienceRepository experienceRepository,
            IUnitOfWork unitOfWork)
        {
            _experienceRepository = experienceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ExperienceDto> Handle(UpdateExperienceCommand request, CancellationToken cancellationToken)
        {
            var experience = await _experienceRepository.GetByIdAsync(request.ExperienceId)
                ?? throw new BadRequestException($"Experience with ID {request.ExperienceId} not found.");

            experience.UpdatedAt = DateTime.UtcNow;

            _experienceRepository.Update(experience);
            await _unitOfWork.SaveChangeAsync();

            return new ExperienceDto
            {
                Id = experience.Id,
                HostId = experience.HostId,
                Title = experience.Title,
                Description = experience.Description,
                Location = new LocationDto
                {
                    Latitude = experience.Location.Y,
                    Longitude = experience.Location.X
                },
                Price = experience.Price,
                Duration = experience.Duration,
                MaxParticipants = experience.MaxParticipants,
                Category = experience.Category.ToString(),
                Amenities = experience.Amenities,
                ActivityLevel = experience.ActivityLevel.ToString(),
                SkillLevel = experience.SkillLevel.ToString(),
                MinAge = experience.MinAge,
                Accessibility = experience.Accessibility,
                Status = experience.Status.ToString(),
                CancellationPolicy = experience.CancellationPolicy,
                MeetingPoint = experience.MeetingPoint,
                Language = experience.Language,
                CreatedAt = experience.CreatedAt,
                UpdatedAt = experience.UpdatedAt
            };
        }
    }
}
