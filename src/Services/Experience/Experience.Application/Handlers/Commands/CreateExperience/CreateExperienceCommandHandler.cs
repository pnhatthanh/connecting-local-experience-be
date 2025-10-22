using BuildingBlocks.Domain.Interfaces;
using Experience.Application.Dtos;
using Experience.Application.Interfaces;
using Experience.Domain.Entities;
using Experience.Domain.Enums;
using Experience.Domain.Repositories;
using MediatR;
using NetTopologySuite.Geometries;

namespace Experience.Application.Handlers.Commands.CreateExperience
{
    public class CreateExperienceCommandHandler : IRequestHandler<CreateExperienceCommand, ExperienceDto>
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoService _photoService;

        public CreateExperienceCommandHandler(
            IExperienceRepository experienceRepository,
            IUnitOfWork unitOfWork,
            IPhotoService photoService)
        {
            _experienceRepository = experienceRepository;
            _unitOfWork = unitOfWork;
            _photoService = photoService;
        }

        public async Task<ExperienceDto> Handle(CreateExperienceCommand request, CancellationToken cancellationToken)
        {
            var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
            var location = geometryFactory.CreatePoint(new Coordinate(request.Location.Longitude, request.Location.Latitude));

            var experience = new ExperienceEntity
            {
                Id = Guid.NewGuid(),
                HostId = request.HostId,
                Title = request.Title,
                Description = request.Description,
                Location = location,
                Price = request.Price,
                Duration = request.Duration,
                MaxParticipants = request.MaxParticipants,
                Category = Enum.Parse<ExperienceCategory>(request.Category),
                Amenities = request.Amenities,
                ActivityLevel = Enum.Parse<ActivityLevel>(request.ActivityLevel),
                SkillLevel = Enum.Parse<SkillLevel>(request.SkillLevel),
                MinAge = request.MinAge,
                Accessibility = request.Accessibility,
                Status = ExperienceStatus.Draft,
                CancellationPolicy = request.CancellationPolicy,
                MeetingPoint = request.MeetingPoint,
                Language = request.Language,
                CreatedAt = DateTime.UtcNow
            };

            if (request.MediaFiles != null && request.MediaFiles.Any())
            {
                var uploadedMedia = new List<ExperienceMediaEntity>();
                for (int i = 0; i < request.MediaFiles.Count; i++)
                {
                    var file = request.MediaFiles[i];
                    try
                    {
                        var uploadResult = await _photoService.UploadPhotoAsync(file, $"experiences/{experience.Id}");
                        uploadedMedia.Add(new ExperienceMediaEntity
                        {
                            Id = Guid.NewGuid(),
                            ExperienceId = experience.Id,
                            Url = uploadResult.Url,
                            Order = i + 1,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to upload media file: {ex.Message}");
                    }
                }
                experience.Media = uploadedMedia;
            }

            if (request.Itineraries != null && request.Itineraries.Any())
            {
                var itineraries = new List<ExperienceItineraryEntity>();
                foreach (var itinerary in request.Itineraries)
                {
                    string? photoUrl = null;
                    
                    if (itinerary.PhotoFile != null)
                    {
                        try
                        {
                            var uploadResult = await _photoService.UploadPhotoAsync(itinerary.PhotoFile, $"experiences/{experience.Id}/itinerary");
                            photoUrl = uploadResult.Url;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to upload itinerary photo: {ex.Message}");
                        }
                    }

                    itineraries.Add(new ExperienceItineraryEntity
                    {
                        Id = Guid.NewGuid(),
                        ExperienceId = experience.Id,
                        StepNumber = itinerary.StepNumber,
                        PhotoUrl = photoUrl ?? string.Empty,
                        Title = itinerary.Title,
                        Description = itinerary.Description,
                        Location = itinerary.Location != null ? geometryFactory.CreatePoint(new Coordinate(itinerary.Location.Longitude, itinerary.Location.Latitude)) : null,
                        CreatedAt = DateTime.UtcNow
                    });
                }
                experience.Itineraries = itineraries;
            }

            await _experienceRepository.AddAsync(experience);
            await _unitOfWork.SaveChangeAsync();

            return new ExperienceDto
            {
                Id = experience.Id,
                HostId = experience.HostId,
                Title = experience.Title,
                Description = experience.Description,
                Location = new LocationDto { Latitude = experience.Location.Y, Longitude = experience.Location.X },
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
                UpdatedAt = experience.UpdatedAt,
                Media = experience.Media?.Select(m => new ExperienceMediaDto { Id = m.Id, Url = m.Url, Order = m.Order }).ToList(),
                Itineraries = experience.Itineraries?.Select(i => new ExperienceItineraryDto { Id = i.Id, StepNumber = i.StepNumber, PhotoUrl = i.PhotoUrl, Title = i.Title, Description = i.Description, Location = i.Location != null ? new LocationDto { Latitude = i.Location.Y, Longitude = i.Location.X } : null }).ToList()
            };
        }
    }
}
