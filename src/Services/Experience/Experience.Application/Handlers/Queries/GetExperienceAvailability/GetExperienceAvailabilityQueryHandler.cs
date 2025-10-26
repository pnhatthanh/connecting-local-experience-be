using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Specifications;
using Experience.Application.Dtos;
using Experience.Application.Utils;
using Experience.Domain.Enums;
using Experience.Domain.Repositories;
using Experience.Domain.Specifications;
using MediatR;

namespace Experience.Application.Handlers.Queries.GetExperienceAvailability
{
    public class GetExperienceAvailabilityQueryHandler : IRequestHandler<GetExperienceAvailabilityQuery, List<ExperienceScheduleSlotDto>>
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly IExperienceScheduleSlotRepository _slotRepository;

        public GetExperienceAvailabilityQueryHandler(
            IExperienceRepository experienceRepository,
            IExperienceScheduleSlotRepository slotRepository)
        {
            _experienceRepository = experienceRepository;
            _slotRepository = slotRepository;
        }

        public async Task<List<ExperienceScheduleSlotDto>> Handle(GetExperienceAvailabilityQuery request, CancellationToken cancellationToken)
        {
            var spec = new ExperienceIdSpecification(request.ExperienceId);
            var experience = await _experienceRepository.GetAnyAsync(spec, e => e.Schedule)
                ?? throw new BadRequestException($"Experience with ID {request.ExperienceId} not found "); 
            var slotSpec = new SlotsByScheduleIdsAndDateRangeSpecification(
                new List<Guid> { experience.Schedule.Id }, 
                request.StartDate, 
                request.EndDate);
            var existingSlots = await _slotRepository.GetAllAsync(slotSpec);
            var generatedSlots = SlotGenerator.GenerateSlotsForSchedule(experience.Schedule, request.StartDate, request.EndDate);
            foreach (var slot in generatedSlots)
            {
                var existing = existingSlots.FirstOrDefault(x =>
                    x.Date == slot.Date &&
                    x.StartTime == slot.StartTime);

                if (existing != null)
                {
                    slot.Id = existing.Id;
                    slot.Status = existing.Status.ToString();
                    slot.TotalSlots = existing.TotalSlots;
                    slot.AvailableSlots = existing.AvailableSlots;
                }
                else
                {
                    slot.Status = SlotStatus.Open.ToString();
                    slot.TotalSlots = experience.MaxParticipants;
                    slot.AvailableSlots = experience.MaxParticipants;
                }
            }
            return generatedSlots.OrderBy(s => s.Date).ThenBy(s => s.StartTime).ToList();
        }
    }
}
