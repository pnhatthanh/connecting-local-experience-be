using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Interfaces;
using Experience.Application.Dtos;
using Experience.Application.Utils;
using Experience.Domain.Entities;
using Experience.Domain.Repositories;
using Experience.Domain.Specifications;

namespace Experience.Application.Handlers.Queries.GetExperienceAvailability
{
    public class GetExperienceAvailabilityQueryHandler : IQueryHandler<GetExperienceAvailabilityQuery, ExperienceCalendarDto>
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly IExperienceScheduleRepository _scheduleRepository;
        private readonly IBaseRepository<ExperienceScheduleSlotEntity> _slotRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GetExperienceAvailabilityQueryHandler(
            IExperienceRepository experienceRepository,
            IExperienceScheduleRepository scheduleRepository,
            IBaseRepository<ExperienceScheduleSlotEntity> slotRepository,
            IUnitOfWork unitOfWork)
        {
            _experienceRepository = experienceRepository;
            _scheduleRepository = scheduleRepository;
            _slotRepository = slotRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ExperienceCalendarDto> Handle(GetExperienceAvailabilityQuery request, CancellationToken cancellationToken)
        {
            var experience = await _experienceRepository.GetByIdAsync(request.ExperienceId)
                ?? throw new KeyNotFoundException($"Experience with ID {request.ExperienceId} not found");

            var scheduleSpec = new ScheduleByExperienceSpecification(request.ExperienceId);
            var schedule = await _scheduleRepository.GetBySpecAsync(scheduleSpec)
                ?? throw new KeyNotFoundException($"Schedule not found for experience {request.ExperienceId}");

            var potentialSlots = SlotGenerator.GenerateSlotsForSchedule(
                schedule,
                request.StartDate,
                request.EndDate
            );

            var scheduleIds = new List<Guid> { schedule.Id };
            var existingSlotsSpec = new SlotsByScheduleIdsAndDateRangeSpecification(
                scheduleIds,
                request.StartDate,
                request.EndDate
            );
            var existingSlots = await _slotRepository.GetAllAsync(existingSlotsSpec);
            var existingSlotsDict = existingSlots.ToDictionary(s => $"{s.Date}_{s.StartTime}_{s.EndTime}");

            var availabilityList = new List<ExperienceAvailabilityDto>();

            foreach (var potentialSlot in potentialSlots)
            {
                var key = $"{potentialSlot.Date}_{potentialSlot.StartTime}_{potentialSlot.EndTime}";
                
                int spotsAvailable;
                if (existingSlotsDict.TryGetValue(key, out var existingSlot))
                {
                    spotsAvailable = existingSlot.AvailableSlots;
                }
                else
                {
                    spotsAvailable = experience.MaxParticipants;
                }

                availabilityList.Add(new ExperienceAvailabilityDto
                {
                    Date = potentialSlot.Date,
                    StartTime = potentialSlot.StartTime,
                    EndTime = potentialSlot.EndTime,
                    SpotsAvailable = spotsAvailable
                });
            }

            var calendar = availabilityList
                .GroupBy(a => a.Date)
                .Select(g => new DateAvailabilityDto
                {
                    Date = g.Key,
                    DayOfWeek = g.Key.DayOfWeek.ToString(),
                    TotalSpotsAvailable = g.Sum(x => x.SpotsAvailable),
                    TimeSlots = g.Select(x =>
                    {
                        var key = $"{x.Date}_{x.StartTime}_{x.EndTime}";
                        var slotId = existingSlotsDict.TryGetValue(key, out var slot) ? slot.Id : (Guid?)null;
                        
                        return new TimeSlotAvailabilityDto
                        {
                            StartTime = x.StartTime,
                            EndTime = x.EndTime,
                            SpotsAvailable = x.SpotsAvailable,
                            SlotId = slotId
                        };
                    }).ToList()
                })
                .OrderBy(d => d.Date)
                .ToList();

            return new ExperienceCalendarDto
            {
                Calendar = calendar
            };
        }
    }
}
