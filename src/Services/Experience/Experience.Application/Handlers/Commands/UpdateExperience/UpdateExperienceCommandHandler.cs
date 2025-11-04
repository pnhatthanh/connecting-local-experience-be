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
using System.Collections.Concurrent;

namespace Experience.Application.Handlers.Commands.UpdateExperience
{
    public class UpdateExperienceCommandHandler : ICommandHandler<UpdateExperienceCommand, ExperienceDto>
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly ICurrentUserService _currentUserService;
        private readonly GeometryFactory _geometryFactory = new(new PrecisionModel(), 4326);

        public UpdateExperienceCommandHandler(IExperienceRepository experienceRepository, IUnitOfWork unitOfWork,
            IMapper mapper, IPhotoService photoService, ICurrentUserService currentUserService)
        {
            _experienceRepository = experienceRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photoService = photoService;
            _currentUserService = currentUserService;
        }

        public async Task<ExperienceDto> Handle(UpdateExperienceCommand request, CancellationToken cancellationToken)
        {
            var experience = await LoadExperienceAsync(request.ExperienceId, cancellationToken);
            ValidateOwnership(experience);
            UpdateBasicInfo(experience, request);
            await UpdateScheduleAsync(experience, request, cancellationToken);
            await UpdateMediaAsync(experience, request, cancellationToken);
            await UpdateItinerariesAsync(experience, request, cancellationToken);

            _experienceRepository.Update(experience);
            await _unitOfWork.SaveChangeAsync();

            return _mapper.Map<ExperienceDto>(experience);
        }

        private async Task<ExperienceEntity> LoadExperienceAsync(Guid experienceId, CancellationToken ct)
        {
            var spec = new ExperienceIdSpecification(experienceId);
            var experience = await _experienceRepository.GetBySpecAsync(spec,
                e => e.Category,
                e => e.Media,
                e => e.Schedule,
                e => e.Itineraries) 
            ?? throw new BadRequestException($"Experience with ID {experienceId} not found.");
            return experience;
        }
        private void ValidateOwnership(ExperienceEntity experience)
        {
            if (experience.HostId != _currentUserService.UserId)
                throw new ForbiddenException("You are not authorized to update this experience.");
        }
        private void UpdateBasicInfo(ExperienceEntity experience, UpdateExperienceCommand request)
        {
            experience.Title = request.Title;
            experience.Description = request.Description;
            experience.Address = request.Address;
            experience.Location = _geometryFactory.CreatePoint(new Coordinate(request.Location.Longitude, request.Location.Latitude));
            experience.District = request.District;
            experience.City = request.City;
            experience.Country = request.Country;
            experience.AdultPrice = request.AdultPrice;
            experience.ChildPrice = request.ChildPrice;
            experience.Duration = request.Duration;
            experience.MaxParticipants = request.MaxParticipants;
            experience.CategoryId = request.CategoryId;
            experience.ActivityLevel = Enum.Parse<ActivityLevel>(request.ActivityLevel, true);
            experience.SkillLevel = Enum.Parse<SkillLevel>(request.SkillLevel, true);
            experience.MinAge = request.MinAge;
            experience.CancellationPolicy = Enum.Parse<CancellationPolicyType>(request.CancellationPolicy, true);
            experience.Language = request.Language;
            experience.MeetingPoint = _geometryFactory.CreatePoint(new Coordinate(request.MeetingPoint.Longitude, request.MeetingPoint.Latitude));
        }
        private async Task UpdateScheduleAsync(ExperienceEntity experience, UpdateExperienceCommand request, CancellationToken ct)
        {
            if (experience.Schedule == null)
            {
                if (request.RecurrenceType == null) return;
                experience.Schedule = new ExperienceScheduleEntity();
            }
            experience.Schedule.RecurrenceType = Enum.Parse<RecurrenceType>(request.RecurrenceType!, true);
            experience.Schedule.DaysOfWeek = request.DaysOfWeek;
            experience.Schedule.StartDate = request.StartDate;
            experience.Schedule.EndDate = request.EndDate;

            experience.Schedule.TimeSlots = request.TimeSlots?
                .Select(t => new ScheduleTimeSlot
                {
                    StartTime = t.StartTime,
                    EndTime = t.EndTime
                })
                .ToList() ?? new List<ScheduleTimeSlot>();
        }
        private async Task UpdateMediaAsync(ExperienceEntity experience, UpdateExperienceCommand request, CancellationToken ct)
        {
            experience.Media ??= new List<ExperienceMediaEntity>();

            var keepMediaIds = request.KeepMediaIds?.ToHashSet() ?? new HashSet<Guid>();

            var mediaToDelete = experience.Media
                .Where(m => !keepMediaIds.Contains(m.Id))
                .ToList();

            if (mediaToDelete.Any())
            {
                await DeleteMediaImagesAsync(mediaToDelete, ct);
                foreach (var media in mediaToDelete)
                    experience.Media.Remove(media);
            }
            else if (keepMediaIds.Count == 0)
            {
                await DeleteMediaImagesAsync(experience.Media, ct);
                experience.Media.Clear();
            }

            if (request.NewMediaFiles?.Any() == true)
            {
                var startOrder = experience.Media.Any() ? experience.Media.Max(m => m.Order) : 0;
                var newMedia = await UploadMediaFilesAsync(experience.Id, request.NewMediaFiles, startOrder, ct);
                foreach (var media in newMedia)
                    experience.Media.Add(media);
            }
        }

        private async Task UpdateItinerariesAsync(ExperienceEntity experience, UpdateExperienceCommand request, CancellationToken ct)
        {
            experience.Itineraries ??= new List<ExperienceItineraryEntity>();

            if (request.Itineraries?.Any() != true)
            {
                await DeleteAllItinerariesAsync(experience.Itineraries, ct);
                experience.Itineraries.Clear();
                return;
            }
            var requestIds = request.Itineraries
                .Where(i => i.Id.HasValue)
                .Select(i => i.Id!.Value)
                .ToHashSet();
            var itinerariesToDelete = experience.Itineraries
                .Where(i => !requestIds.Contains(i.Id))
                .ToList();

            if (itinerariesToDelete.Any())
            {
                await DeleteItineraryImagesAsync(itinerariesToDelete, ct);
                foreach (var it in itinerariesToDelete)
                    experience.Itineraries.Remove(it);
            }
            foreach (var reqIt in request.Itineraries)
            {
                if (reqIt.Id.HasValue)
                {
                    var existing = experience.Itineraries.FirstOrDefault(i => i.Id == reqIt.Id.Value);
                    if (existing != null)
                    {
                        await UpdateExistingItineraryAsync(existing, reqIt, experience.Id, ct);
                    }
                }
                else
                {
                    var newItinerary = await CreateNewItineraryAsync(reqIt, experience.Id, ct);
                    experience.Itineraries.Add(newItinerary);
                }
            }
        }

        private async Task UpdateExistingItineraryAsync(ExperienceItineraryEntity existing, 
            UpdateExperienceItineraryDto request, Guid experienceId, CancellationToken ct)
        {
            existing.StepNumber = request.StepNumber;
            existing.Title = request.Title;
            existing.Description = request.Description;
            if (request.PhotoFile != null)
            {
                if (!string.IsNullOrEmpty(existing.PhotoUrl))
                {
                    await _photoService.DeleteImageAsync(existing.PhotoUrl);
                }

                var uploadPath = $"experiences/{experienceId}/itinerary";
                existing.PhotoUrl = await _photoService.UploadImageAsync(request.PhotoFile, uploadPath);
            }
        }

        private async Task<ExperienceItineraryEntity> CreateNewItineraryAsync(UpdateExperienceItineraryDto request, 
            Guid experienceId, CancellationToken ct)
        {
            string? photoUrl = null;
            if (request.PhotoFile != null)
            {
                var uploadPath = $"experiences/{experienceId}/itinerary";
                photoUrl = await _photoService.UploadImageAsync(request.PhotoFile, uploadPath);
            }
            return new ExperienceItineraryEntity
            {
                ExperienceId = experienceId,
                StepNumber = request.StepNumber,
                Title = request.Title,
                Description = request.Description,
                PhotoUrl = photoUrl ?? string.Empty
            };
        }

        private async Task DeleteMediaImagesAsync(IEnumerable<ExperienceMediaEntity> media, CancellationToken ct)
        {
            var tasks = media
                .Where(m => !string.IsNullOrEmpty(m.Url))
                .Select(m => SafeDeleteImageAsync(m.Url, ct));

            await Task.WhenAll(tasks);
        }

        private async Task DeleteItineraryImagesAsync(IEnumerable<ExperienceItineraryEntity> itineraries, CancellationToken ct)
        {
            var tasks = itineraries
                .Where(i => !string.IsNullOrEmpty(i.PhotoUrl))
                .Select(i => SafeDeleteImageAsync(i.PhotoUrl, ct));
            await Task.WhenAll(tasks);
        }

        private async Task DeleteAllItinerariesAsync(ICollection<ExperienceItineraryEntity> itineraries, CancellationToken ct)
        {
            if (!itineraries.Any()) 
                return;
            await DeleteItineraryImagesAsync(itineraries, ct);
        }

        private async Task SafeDeleteImageAsync(string url, CancellationToken ct)
        {
            try
            {
                await _photoService.DeleteImageAsync(url);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[WARN] Failed to delete image {url}: {ex.Message}");
            }
        }

        private async Task<List<ExperienceMediaEntity>> UploadMediaFilesAsync(
            Guid experienceId, List<IFormFile> files, int startOrder, CancellationToken ct)
        {
            var uploadedMedia = new ConcurrentBag<ExperienceMediaEntity>();
            var uploadPath = $"experiences/{experienceId}";
            var uploadTasks = files.Select(async (file, index) =>
            {
                try
                {
                    var url = await _photoService.UploadImageAsync(file, uploadPath);
                    uploadedMedia.Add(new ExperienceMediaEntity
                    {
                        ExperienceId = experienceId,
                        Url = url,
                        Order = startOrder + index + 1
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ERROR] Upload failed for file {file.FileName}: {ex.Message}");
                }
            });
            await Task.WhenAll(uploadTasks);
            return uploadedMedia.ToList();
        }
    }
}