using BuildingBlocks.Application.CQRS.Query;
using Experience.Domain.Enums;
using Experience.Domain.Repositories;
using Experience.Domain.Specifications;
using Microsoft.Extensions.Logging;

namespace Experience.Application.Handlers.Queries.ValidateBookingAvailability
{
    public class ValidateBookingAvailabilityQueryHandler : IQueryHandler<ValidateBookingAvailabilityQuery, bool>
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly IExperienceScheduleRepository _scheduleRepository;
        private readonly IExperienceScheduleSlotRepository _slotRepository;
        private readonly ILogger<ValidateBookingAvailabilityQueryHandler> _logger;

        public ValidateBookingAvailabilityQueryHandler(
            IExperienceRepository experienceRepository,
            IExperienceScheduleRepository scheduleRepository,
            ILogger<ValidateBookingAvailabilityQueryHandler> logger,
            IExperienceScheduleSlotRepository slotRepository)
        {
            _experienceRepository = experienceRepository;
            _scheduleRepository = scheduleRepository;
            _slotRepository = slotRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(ValidateBookingAvailabilityQuery request, CancellationToken cancellationToken)
        {

            // Get experience
            var experience = await _experienceRepository.GetByIdAsync(request.ExperienceId);
            if (experience == null)
            {
                _logger.LogWarning("ValidateBookingAvailabilityQueryHandler: Experience with ID {ExperienceId} not found.", request.ExperienceId);
                return false;
            }

            // Check if experience is approved
            if (experience.Status != ExperienceStatus.Approved)
            {
                _logger.LogWarning("ValidateBookingAvailabilityQueryHandler: Experience with ID {ExperienceId} is not approved.", request.ExperienceId);
                return false;
            }

            // Get schedule
            var scheduleSpec = new ScheduleByExperienceSpecification(request.ExperienceId);
            var schedule = await _scheduleRepository.GetBySpecAsync(scheduleSpec);
            if (schedule == null)
            {
                _logger.LogWarning("ValidateBookingAvailabilityQueryHandler: Schedule for Experience ID {ExperienceId} not found.", request.ExperienceId);
                return false;
            }

            if (request.Date < schedule.StartDate || (schedule.EndDate.HasValue && request.Date > schedule.EndDate.Value))
            {
                _logger.LogWarning("ValidateBookingAvailabilityQueryHandler: Requested date {Date} is outside the schedule range for Experience ID {ExperienceId}.", request.Date, request.ExperienceId);
                return false;
            }

            var dayOfWeek = request.Date.DayOfWeek;
            if (!schedule.DaysOfWeek.Contains(dayOfWeek))
            {
                _logger.LogWarning("ValidateBookingAvailabilityQueryHandler: Requested date {Date} falls on {DayOfWeek}, which is not available in the schedule for Experience ID {ExperienceId}.", request.Date, dayOfWeek, request.ExperienceId);
                return false;
            }

            // Check if the requested time slot exists in schedule
            var isValidTimeSlot = schedule.TimeSlots.Any(ts =>
                ts.StartTime == request.StartTime &&
                ts.EndTime == request.EndTime);

            if (!isValidTimeSlot)
            {
                _logger.LogWarning("ValidateBookingAvailabilityQueryHandler: Requested time slot {StartTime} - {EndTime} is not available in the schedule for Experience ID {ExperienceId}.", request.StartTime, request.EndTime, request.ExperienceId);
                return false;
            }

            // Calculate total participants needed
            int totalParticipants = request.Adults + request.Children;

            // Check max participants
            if (totalParticipants > experience.MaxParticipants)
            {
                _logger.LogWarning("ValidateBookingAvailabilityQueryHandler: Total participants {TotalParticipants} exceed max participants {MaxParticipants} for Experience ID {ExperienceId}.", totalParticipants, experience.MaxParticipants, request.ExperienceId);
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
                _logger.LogInformation("ValidateBookingAvailabilityQueryHandler: Found existing slot for Experience ID {ExperienceId} on {Date} at {StartTime} - {EndTime} with {AvailableSlots} available slots.", request.ExperienceId, request.Date, request.StartTime, request.EndTime, existingSlot.AvailableSlots);
                availableSlots = existingSlot.AvailableSlots;
            }
            else
            {
                _logger.LogInformation("ValidateBookingAvailabilityQueryHandler: No existing slot found for Experience ID {ExperienceId} on {Date} at {StartTime} - {EndTime}. Using max participants as available slots.", request.ExperienceId, request.Date, request.StartTime, request.EndTime);
                // No bookings yet, use max participants
                availableSlots = experience.MaxParticipants;
            }
            _logger.LogInformation("ValidateBookingAvailabilityQueryHandler: Available slots for Experience ID {ExperienceId} on {Date} at {StartTime} - {EndTime} is {AvailableSlots}.", request.ExperienceId, request.Date, request.StartTime, request.EndTime, availableSlots);
            // Check if enough slots available
            return availableSlots >= totalParticipants;
        }
    }
}
