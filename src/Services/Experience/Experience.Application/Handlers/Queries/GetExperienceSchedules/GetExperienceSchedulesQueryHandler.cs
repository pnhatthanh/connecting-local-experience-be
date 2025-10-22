using Experience.Application.Dtos;
using Experience.Domain.Repositories;
using MediatR;

namespace Experience.Application.Handlers.Queries.GetExperienceSchedules
{
    public class GetExperienceSchedulesQueryHandler : IRequestHandler<GetExperienceSchedulesQuery, List<ExperienceScheduleDto>>
    {
        private readonly IExperienceScheduleRepository _scheduleRepository;

        public GetExperienceSchedulesQueryHandler(IExperienceScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }

        public async Task<List<ExperienceScheduleDto>> Handle(GetExperienceSchedulesQuery request, CancellationToken cancellationToken)
        {
            var schedules = await _scheduleRepository.GetAllAsync(
                specification: null,
                includes: s => s.Slots
            );

            var filteredSchedules = schedules.Where(s => s.ExperienceId == request.ExperienceId).ToList();

            if (!filteredSchedules.Any())
            {
                return new List<ExperienceScheduleDto>();
            }

            // Map to DTOs
            var scheduleDtos = filteredSchedules.Select(schedule => new ExperienceScheduleDto
            {
                Id = schedule.Id,
                ExperienceId = schedule.ExperienceId,
                IsRecurring = schedule.IsRecurring,
                RecurringPattern = schedule.RecurringPattern,
                StartDate = schedule.StartDate,
                EndDate = schedule.EndDate,
                Timezone = schedule.Timezone,
                Slots = schedule.Slots?.Select(slot => new ExperienceScheduleSlotDto
                {
                    Id = slot.Id,
                    ScheduleId = slot.ScheduleId,
                    Date = slot.Date,
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime,
                    TotalSlots = slot.TotalSlots,
                    AvailableSlots = slot.AvailableSlots,
                    Status = slot.Status.ToString()
                }).OrderBy(s => s.Date).ThenBy(s => s.StartTime).ToList()
            }).OrderBy(s => s.StartDate).ToList();

            return scheduleDtos;
        }
    }
}

