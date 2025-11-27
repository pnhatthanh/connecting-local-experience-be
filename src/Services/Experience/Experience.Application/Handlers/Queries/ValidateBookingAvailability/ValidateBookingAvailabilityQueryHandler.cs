using BuildingBlocks.Application.CQRS.Query;
using Experience.Domain.Enums;
using Experience.Domain.Repositories;
using Experience.Domain.Specifications;

namespace Experience.Application.Handlers.Queries.ValidateBookingAvailability
{
    public class ValidateBookingAvailabilityQueryHandler : IQueryHandler<ValidateBookingAvailabilityQuery, bool>
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly IExperienceScheduleRepository _scheduleRepository;
        private readonly IExperienceScheduleSlotRepository _slotRepository;

        public ValidateBookingAvailabilityQueryHandler(
            IExperienceRepository experienceRepository,
            IExperienceScheduleRepository scheduleRepository,
            IExperienceScheduleSlotRepository slotRepository)
        {
            _experienceRepository = experienceRepository;
            _scheduleRepository = scheduleRepository;
            _slotRepository = slotRepository;
        }

        public async Task<bool> Handle(ValidateBookingAvailabilityQuery request, CancellationToken cancellationToken)
        {
            // Get experience
            var experience = await _experienceRepository.GetByIdAsync(request.ExperienceId);
            if (experience == null)
            {
                return false;
            }

            // Check if experience is approved
            if (experience.Status != ExperienceStatus.Approved)
            {
                return false;
            }

            // Get schedule
            var scheduleSpec = new ScheduleByExperienceSpecification(request.ExperienceId);
            var schedule = await _scheduleRepository.GetBySpecAsync(scheduleSpec);
            if (schedule == null)
            {
                return false;
            }

            if (request.Date < schedule.StartDate || (schedule.EndDate.HasValue && request.Date > schedule.EndDate.Value))
            {
                return false;
            }

            var dayOfWeek = request.Date.DayOfWeek;
            if (!schedule.DaysOfWeek.Contains(dayOfWeek))
            {
                return false;
            }

            // Check if the requested time slot exists in schedule
            var isValidTimeSlot = schedule.TimeSlots.Any(ts =>
                ts.StartTime == request.StartTime &&
                ts.EndTime == request.EndTime);

            if (!isValidTimeSlot)
            {
                return false;
            }

            // Calculate total participants needed
            int totalParticipants = request.Adults + request.Children;

            // Check max participants
            if (totalParticipants > experience.MaxParticipants)
            {
                return false;
            }

            // Check available slots for this specific date and time
            var scheduleIds = new List<Guid> { schedule.Id };
            var existingSlotsSpec = new SlotsByScheduleIdsAndDateRangeSpecification(
                scheduleIds,
                request.Date,
                request.Date
            );
            var existingSlots = await _slotRepository.GetAllAsync(existingSlotsSpec);

            var existingSlot = existingSlots.FirstOrDefault(s =>
                s.Date == request.Date &&
                s.StartTime == request.StartTime &&
                s.EndTime == request.EndTime);

            int availableSlots;
            if (existingSlot != null)
            {
                availableSlots = existingSlot.AvailableSlots;
            }
            else
            {
                // No bookings yet, use max participants
                availableSlots = experience.MaxParticipants;
            }

            // Check if enough slots available
            return availableSlots >= totalParticipants;
        }
    }
}
