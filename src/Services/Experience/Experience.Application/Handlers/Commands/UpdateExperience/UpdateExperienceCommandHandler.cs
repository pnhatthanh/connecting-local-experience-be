using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using Experience.Application.Dtos;
using Experience.Domain.Entities;
using Experience.Domain.Repositories;
using Experience.Domain.Specifications;
using MapsterMapper;
using MediatR;
using NetTopologySuite.Geometries;

namespace Experience.Application.Handlers.Commands.UpdateExperience
{
    public class UpdateExperienceCommandHandler : IRequestHandler<UpdateExperienceCommand, ExperienceDto>
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateExperienceCommandHandler(
            IExperienceRepository experienceRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _experienceRepository = experienceRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ExperienceDto> Handle(UpdateExperienceCommand request, CancellationToken cancellationToken)
        {
            var spec = new ExperienceIdSpecification(request.ExperienceId);
            var experience = await _experienceRepository.GetAnyAsync(spec, 
                e => e.Category, 
                e => e.Media, 
                e => e.Schedule, 
                e => e.Itineraries)
                ?? throw new BadRequestException($"Experience with ID {request.ExperienceId} not found.");

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
            experience.MinAge = request.MinAge;
            experience.CancellationPolicy = request.CancellationPolicy;
            experience.MeetingLocation = request.MeetingLocation;

            var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
            experience.MeetingPoint = geometryFactory.CreatePoint(new Coordinate(request.MeetingPoint.Longitude, request.MeetingPoint.Latitude));
            if (experience.Schedule != null)
            {
                experience.Schedule.RecurrenceType = request.RecurrenceType;
                experience.Schedule.DaysOfWeek = request.DaysOfWeek;
                experience.Schedule.TimeSlots = request.TimeSlots.Select(t => new ScheduleTimeSlot
                {
                    StartTime = t.StartTime,
                    EndTime = t.EndTime
                }).ToList();
                experience.Schedule.StartDate = request.ScheduleStartDate;
                experience.Schedule.EndDate = request.ScheduleEndDate;
                experience.Schedule.UpdatedAt = DateTime.UtcNow;
            }
            experience.UpdatedAt = DateTime.UtcNow;
            _experienceRepository.Update(experience);
            await _unitOfWork.SaveChangeAsync();
            var updatedExperience = await _experienceRepository.GetAnyAsync(spec, 
                e => e.Category, 
                e => e.Media, 
                e => e.Schedule, 
                e => e.Itineraries)
                ?? throw new BadRequestException("Failed to reload updated experience.");

            return _mapper.Map<ExperienceDto>(updatedExperience);
        }
    }
}
