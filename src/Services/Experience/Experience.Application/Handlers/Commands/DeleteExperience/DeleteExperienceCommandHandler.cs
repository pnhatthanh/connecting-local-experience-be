using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using Experience.Application.Interfaces;
using Experience.Domain.Repositories;
using Experience.Domain.Specifications;

namespace Experience.Application.Handlers.Commands.DeleteExperience
{
    public class DeleteExperienceCommandHandler : ICommandHandler<DeleteExperienceCommand, bool>
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly IUnitOfWork _unitOfWork; 
        private readonly ICurrentUserService _currentUserService;
        private readonly IPhotoService _photoService;

        public DeleteExperienceCommandHandler(IExperienceRepository experienceRepository, IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService, IPhotoService photoService)
        {
            _experienceRepository = experienceRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _photoService = photoService;
        }
        public async Task<bool> Handle(DeleteExperienceCommand request, CancellationToken cancellationToken)
        {
            var specification = new ExperienceIdSpecification(request.Id);
            var experience = await _experienceRepository.GetBySpecAsync(specification, e => e.Media, e => e.Itineraries)  
                ?? throw new BadRequestException($"Experience with ID {request.Id} not found.");
            if (experience.HostId != _currentUserService.UserId)
                throw new ForbiddenException("You are not authorized to delete this experience.");

            if (experience.Media != null && experience.Media.Any())
            {
                foreach (var media in experience.Media)
                {
                    try
                    {
                        await _photoService.DeleteImageAsync(media.Url);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to delete media {media.Url}: {ex.Message}");
                    }
                }
            }

            if (experience.Itineraries != null && experience.Itineraries.Any())
            {
                foreach (var itinerary in experience.Itineraries)
                {
                    if (!string.IsNullOrEmpty(itinerary.PhotoUrl))
                    {
                        try
                        {
                            await _photoService.DeleteImageAsync(itinerary.PhotoUrl);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to delete itinerary image {itinerary.PhotoUrl}: {ex.Message}");
                        }
                    }
                }
            }
            _experienceRepository.Delete(experience);
            await _unitOfWork.SaveChangeAsync();
            
            return true;
        }
    }
}
