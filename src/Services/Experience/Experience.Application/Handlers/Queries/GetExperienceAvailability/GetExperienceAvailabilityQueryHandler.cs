using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Domain.Exceptions;
using Experience.Application.Dtos;
using Experience.Application.Utils;
using Experience.Domain.Repositories;
using Experience.Domain.Specifications;

namespace Experience.Application.Handlers.Queries.GetExperienceAvailability
{
    public class GetExperienceAvailabilityQueryHandler : IQueryHandler<GetExperienceAvailabilityQuery, List<ExperienceAvailabilityDto>>
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly IExperienceScheduleRepository _scheduleRepository;
        private readonly IExperienceScheduleSlotRepository _slotRepository;

        public GetExperienceAvailabilityQueryHandler(
            IExperienceRepository experienceRepository,
            IExperienceScheduleRepository scheduleRepository,
            IExperienceScheduleSlotRepository slotRepository)
        {
            _experienceRepository = experienceRepository;
            _scheduleRepository = scheduleRepository;
            _slotRepository = slotRepository;
        }

        public async Task<List<ExperienceAvailabilityDto>> Handle(GetExperienceAvailabilityQuery request, CancellationToken cancellationToken)
        {
            var experience = await _experienceRepository.GetByIdAsync(request.ExperienceId)
                ?? throw new BadRequestException($"Experience with ID {request.ExperienceId} not found");

            var scheduleSpec = new ScheduleByExperienceSpecification(request.ExperienceId);
            var schedule = await _scheduleRepository.GetBySpecAsync(scheduleSpec)
                ?? throw new BadRequestException($"Schedule not found for experience {request.ExperienceId}");

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

            return availabilityList.OrderBy(x => x.Date)
                    .ThenBy(x => x.StartTime).ToList();
        }
    }
}
