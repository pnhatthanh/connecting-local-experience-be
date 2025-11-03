using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using Experience.Application.Dtos;
using Experience.Application.Interfaces;
using Experience.Domain.Entities;
using Experience.Domain.Enums;
using Experience.Domain.Repositories;
using Experience.Domain.Specifications;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using NetTopologySuite.Geometries;

namespace Experience.Application.Handlers.Commands.UpdateExperience
{
    public class UpdateExperienceCommandHandler : ICommandHandler<UpdateExperienceCommand, ExperienceDto>
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly ICurrentUserService _currentUserService;

        public UpdateExperienceCommandHandler(
            IExperienceRepository experienceRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IPhotoService photoService,
            ICurrentUserService currentUserService)
        {
            _experienceRepository = experienceRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photoService = photoService;
            _currentUserService = currentUserService;
        }

        public async Task<ExperienceDto> Handle(UpdateExperienceCommand request, CancellationToken cancellationToken)
        {
            var spec = new ExperienceIdSpecification(request.ExperienceId);
            var experience = await _experienceRepository.GetBySpecAsync(spec,
                e => e.Category,
                e => e.Media,
                e => e.Schedule,
                e => e.Itineraries)
                ?? throw new BadRequestException($"Experience with ID {request.ExperienceId} not found.");
            if (experience.HostId != _currentUserService.UserId)
                throw new ForbiddenException("You are not authorized to update this experience.");
            experience.Title = request.Title;
            experience.Description = request.Description;
            experience.Address = request.Address;
            experience.District = request.District;
            experience.City = request.City;
            experience.Country = request.Country;
            experience.AdultPrice = request.AdultPrice;
            experience.ChildPrice = request.ChildPrice;
            experience.Duration = request.Duration;
            experience.MaxParticipants = request.MaxParticipants;
            experience.CategoryId = request.CategoryId;
            experience.ActivityLevel = Enum.Parse<ActivityLevel>(request.ActivityLevel);
            experience.SkillLevel = Enum.Parse<SkillLevel>(request.SkillLevel);
            experience.MinAge = request.MinAge;
            experience.CancellationPolicy = Enum.Parse<CancellationPolicyType>(request.CancellationPolicy);
            experience.MeetingLocation = request.MeetingLocation;
            experience.Language = request.Language;

            var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
            experience.MeetingPoint = geometryFactory.CreatePoint(new Coordinate(request.MeetingPoint.Longitude, request.MeetingPoint.Latitude));
            
            if (experience.Schedule != null)
            {
                experience.Schedule.RecurrenceType = Enum.Parse<RecurrenceType>(request.RecurrenceType);
                experience.Schedule.DaysOfWeek = request.DaysOfWeek;
                experience.Schedule.TimeSlots = request.TimeSlots.Select(t => new ScheduleTimeSlot
                {
                    StartTime = t.StartTime,
                    EndTime = t.EndTime
                }).ToList();
                experience.Schedule.StartDate = request.StartDate;
                experience.Schedule.EndDate = request.EndDate;
                experience.Schedule.UpdatedAt = DateTime.UtcNow;
            }
            
            if (request.MediaFiles?.Any() == true)
            {
                if (experience.Media?.Any() == true)
                {
                    experience.Media.Clear();
                }
                
                experience.Media = await UploadMediaFilesAsync(experience.Id, request.MediaFiles);
            }
            
            if (request.Itineraries?.Any() == true)
            {
                if (experience.Itineraries?.Any() == true)
                {
                    experience.Itineraries.Clear();
                }
                experience.Itineraries = await UploadItinerariesAsync(experience.Id, request.Itineraries);
            }
            
            experience.UpdatedAt = DateTime.UtcNow;
            _experienceRepository.Update(experience);
            await _unitOfWork.SaveChangeAsync();

            return _mapper.Map<ExperienceDto>(experience);
        }
        
        private async Task<List<ExperienceMediaEntity>> UploadMediaFilesAsync(Guid experienceId, List<IFormFile> mediaFiles)
        {
            var uploadedMedia = new List<ExperienceMediaEntity>();
            for (int i = 0; i < mediaFiles.Count; i++)
            {
                var file = mediaFiles[i];
                try
                {
                    var uploadPath = $"experiences/{experienceId}"; 
                    var uploadedUrl = await _photoService.UploadImageAsync(file, uploadPath);
                    uploadedMedia.Add(new ExperienceMediaEntity
                    {
                        Id = Guid.NewGuid(),
                        ExperienceId = experienceId,
                        Url = uploadedUrl,
                        Order = i + 1, 
                        CreatedAt = DateTime.UtcNow
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to upload media file for experience {experienceId}: {ex.Message}");
                }
            }
            return uploadedMedia;
        }

        private async Task<List<ExperienceItineraryEntity>> UploadItinerariesAsync(Guid experienceId, List<UpdateExperienceItineraryDto> itineraries)
        {
            var itineraryEntities = new List<ExperienceItineraryEntity>();
            foreach (var itinerary in itineraries)
            {
                string? photoUrl = null;
                if (itinerary.PhotoFile != null)
                {
                    try
                    {
                        var uploadPath = $"experiences/{experienceId}/itinerary";
                        photoUrl = await _photoService.UploadImageAsync(itinerary.PhotoFile, uploadPath);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to upload itinerary photo for step {itinerary.StepNumber}: {ex.Message}");
                    }
                }
                itineraryEntities.Add(new ExperienceItineraryEntity
                {
                    Id = Guid.NewGuid(),
                    ExperienceId = experienceId,
                    StepNumber = itinerary.StepNumber,
                    PhotoUrl = photoUrl ?? string.Empty,
                    Title = itinerary.Title,
                    Description = itinerary.Description,
                    CreatedAt = DateTime.UtcNow
                });
            }
            return itineraryEntities;
        }
    }
}
