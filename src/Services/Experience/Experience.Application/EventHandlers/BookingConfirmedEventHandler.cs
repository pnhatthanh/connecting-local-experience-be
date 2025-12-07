using BuildingBlocks.Application.EventBus.Abstractions;
using BuildingBlocks.Domain.Interfaces;
using Experience.Application.Events;
using Experience.Application.Handlers.Queries.GetExperiencesByIds;
using Experience.Domain.Entities;
using Experience.Domain.Enums;
using Experience.Domain.Repositories;
using Experience.Domain.Specifications;
using Microsoft.Extensions.Logging;

namespace Experience.Application.EventHandlers
{
    public class BookingConfirmedEventHandler : IIntegrationEventHandler<BookingConfirmedEvent>
    {
        private readonly IExperienceScheduleSlotRepository _scheduleSlotRepository;
        private readonly IExperienceRepository _experienceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BookingConfirmedEventHandler> _logger;

        public BookingConfirmedEventHandler( IExperienceScheduleSlotRepository scheduleSlotRepository, IExperienceRepository experienceRepository,
            IUnitOfWork unitOfWork, ILogger<BookingConfirmedEventHandler> logger)
        {
            _scheduleSlotRepository = scheduleSlotRepository;
            _experienceRepository = experienceRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task HandleAsync(BookingConfirmedEvent @event, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Processing BookingConfirmedEvent for Experience {ExperienceId}, Date {Date}, Time {StartTime}-{EndTime}",
                    @event.ExperienceId, @event.Date, @event.StartTime, @event.EndTime);
                var spec = new ExperienceIdSpecification(@event.ExperienceId);
                var experience = await _experienceRepository.GetBySpecAsync(spec, experience => experience.Schedule);
                if (experience == null)
                {
                    _logger.LogError("Experience {ExperienceId} not found", @event.ExperienceId);
                    return;
                }

                var scheduleSlot = await _scheduleSlotRepository.GetSlotByDateAndTimeAsync(
                    experience.Schedule.Id, 
                    @event.Date, 
                    @event.StartTime, 
                    @event.EndTime
                );

                if (scheduleSlot == null)
                {
                    scheduleSlot = new ExperienceScheduleSlotEntity
                    {
                        ScheduleId = experience.Schedule.Id,
                        Date = @event.Date,
                        StartTime = @event.StartTime,
                        EndTime = @event.EndTime,
                        TotalSlots = experience.MaxParticipants,
                        AvailableSlots = experience.MaxParticipants - @event.TotalParticipants,
                        Status = SlotStatus.Open
                    };
                    await _scheduleSlotRepository.AddAsync(scheduleSlot);
                    
                    _logger.LogInformation("Created new schedule slot for Experience {ExperienceId} on {Date} {StartTime}-{EndTime}. Available slots: {AvailableSlots}/{TotalSlots}",
                        @event.ExperienceId, @event.Date, @event.StartTime, @event.EndTime, scheduleSlot.AvailableSlots, scheduleSlot.TotalSlots);
                }
                else
                {
                    scheduleSlot.AvailableSlots -= @event.TotalParticipants;
                    if (scheduleSlot.AvailableSlots <= 0)
                    {
                        scheduleSlot.Status = SlotStatus.FullyBooked;
                        scheduleSlot.AvailableSlots = 0;
                    }
                    _scheduleSlotRepository.Update(scheduleSlot);
                    _logger.LogInformation("Updated schedule slot for Experience {ExperienceId} on {Date} {StartTime}-{EndTime}. Reduced by {Participants} participants. Available slots: {AvailableSlots}/{TotalSlots}",
                        @event.ExperienceId, @event.Date, @event.StartTime, @event.EndTime, @event.TotalParticipants, scheduleSlot.AvailableSlots, scheduleSlot.TotalSlots);
                }
                await _unitOfWork.SaveChangeAsync();
                _logger.LogInformation("Successfully processed BookingConfirmedEvent for Booking {BookingId}", @event.BookingId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing BookingConfirmedEvent for Booking {BookingId}", @event.BookingId);
            }
        }
    }
}
