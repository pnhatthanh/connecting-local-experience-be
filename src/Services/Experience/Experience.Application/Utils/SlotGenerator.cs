using Experience.Application.Dtos;
using Experience.Domain.Entities;
using Experience.Domain.Enums;

namespace Experience.Application.Utils
{
    public static class SlotGenerator
    {
        public static List<ExperienceScheduleSlotDto> GenerateSlotsForSchedule(
            ExperienceScheduleEntity schedule,
            DateOnly startDate,
            DateOnly endDate)
        {
            var slots = new List<ExperienceScheduleSlotDto>();

            if (schedule.RecurrenceType == RecurrenceType.Once)
            {
                if (schedule.StartDate >= startDate && schedule.StartDate <= endDate)
                {
                    foreach (var timeSlot in schedule.TimeSlots)
                    {
                        slots.Add(new ExperienceScheduleSlotDto
                        {
                            ScheduleId = schedule.Id,
                            Date = schedule.StartDate,
                            StartTime = timeSlot.StartTime,
                            EndTime = timeSlot.EndTime
                        });
                    }
                }
            }
            if (schedule.RecurrenceType == RecurrenceType.Weekly)
            {
                var currentDate = startDate > schedule.StartDate
                    ? startDate
                    : schedule.StartDate;

                var lastDate = endDate < (schedule.EndDate ?? DateOnly.MaxValue)
                    ? endDate
                    : (schedule.EndDate ?? DateOnly.MaxValue);

                while (currentDate <= lastDate)
                {
                    if (schedule.DaysOfWeek.Contains(currentDate.DayOfWeek))
                    {
                        foreach (var timeSlot in schedule.TimeSlots)
                        {
                            slots.Add(new ExperienceScheduleSlotDto
                            {
                                ScheduleId = schedule.Id,
                                Date = currentDate,
                                StartTime = timeSlot.StartTime,
                                EndTime = timeSlot.EndTime
                            });
                        }
                    }
                    currentDate = currentDate.AddDays(1);
                }
            }

            return slots;
        }
    }
}
