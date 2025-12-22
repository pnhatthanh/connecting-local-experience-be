using System.Diagnostics;
using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Application.EventBus.Abstractions;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using Experience.Application.Dtos;
using Experience.Application.Events;
using Experience.Application.Interfaces;
using Experience.Domain.Entities;
using Experience.Domain.Enums;
using Experience.Domain.Repositories;
using MapsterMapper;
using Microsoft.AspNetCore.Http;

namespace Experience.Application.Handlers.Commands.CreateExperience
{
    public class CreateExperienceCommandHandler : ICommandHandler<CreateExperienceCommand, ExperienceDto>
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly IExperienceCategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoService _photoService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IEventBus _eventBus;

        public CreateExperienceCommandHandler(
            IExperienceRepository experienceRepository,
            IExperienceCategoryRepository categoryRepository,
            IUnitOfWork unitOfWork, 
            IPhotoService photoService, 
            ICurrentUserService currentUserService,
            IMapper mapper,
            IEventBus eventBus)
        {
            _experienceRepository = experienceRepository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _photoService = photoService;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _eventBus = eventBus;
        }

        public async Task<ExperienceDto> Handle(CreateExperienceCommand request, CancellationToken cancellationToken)
        {
            var hostId = _currentUserService.UserId;
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId)
                ?? throw new BadRequestException("Invalid category ID");
            var experience = new ExperienceEntity
            {
                Id = Guid.NewGuid(),
                HostId = hostId,
                Title = request.Title,
                Description = request.Description,
                Address = request.Address,
                District = request.District,
                City = request.City,
                Country = request.Country,
                AdultPrice = request.AdultPrice,
                ChildPrice = request.ChildPrice,
                Duration = request.Duration,
                MaxParticipants = request.MaxParticipants,
                CategoryId = request.CategoryId,
                ActivityLevel = Enum.Parse<ActivityLevel>(request.ActivityLevel),
                SkillLevel = Enum.Parse<SkillLevel>(request.SkillLevel),
                MinAge = request.MinAge,
                Status = ExperienceStatus.Pending,
                CancellationPolicy = Enum.Parse<CancellationPolicyType>(request.CancellationPolicy),
                Language = request.Language,
            };
            
            if (request.MediaFiles?.Any() == true)
                experience.Media = await UploadMediaFilesAsync(experience.Id, request.MediaFiles);
            if (request.Itineraries?.Any() == true)
                experience.Itineraries = await UploadItinerariesAsync(experience.Id, request.Itineraries);
            
            experience.Schedule = new ExperienceScheduleEntity
            {
                Id = Guid.NewGuid(),
                ExperienceId = experience.Id,
                RecurrenceType = Enum.Parse<RecurrenceType>(request.RecurrenceType),
                DaysOfWeek = request.DaysOfWeek,
                TimeSlots = request.TimeSlots.Select(t => new ScheduleTimeSlot
                {
                    StartTime = t.StartTime,
                    EndTime = t.EndTime
                }).ToList(),
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                CreatedAt = DateTime.UtcNow
            };
            
            await _experienceRepository.AddAsync(experience);
            await _unitOfWork.SaveChangeAsync();
            
            var experienceCreatedEvent = new ExperienceCreatedEvent(
                experienceId: experience.Id.ToString(),
                hostId: experience.HostId,
                title: experience.Title,
                description: experience.Description,
                maxParticipants: experience.MaxParticipants,
                address: $"{experience.City}, {experience.Country}",
                category: category.Name,
                adultPrice: experience.AdultPrice,
                childPrice: experience.ChildPrice,
                duration: experience.Duration,
                language: experience.Language,
                media: _mapper.Map<List<ExperienceMediaDto>>(experience.Media)
            );
            await _eventBus.PublishAsync(experienceCreatedEvent, cancellationToken);
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
                    Debug.WriteLine($"Failed to upload media file for experience {experienceId}: {ex.Message}");
                }
            }
            return uploadedMedia;
        }

        private async Task<List<ExperienceItineraryEntity>> UploadItinerariesAsync( Guid experienceId, List<CreateExperienceItineraryDto> itineraries)
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
                        Debug.WriteLine($"Failed to upload itinerary photo for step {itinerary.StepNumber}: {ex.Message}");
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