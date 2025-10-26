using Experience.Application.Dtos;
using Experience.Domain.Entities;
using Experience.Domain.Enums;

namespace Experience.Application.Utils
{
    public static class SlotGenerator
    {
        public static List<ExperienceScheduleSlotDto> GenerateSlotsForSchedule(
            ExperienceScheduleEntity schedule,
            DateTime startDate,
            DateTime endDate)
        {
            var slots = new List<ExperienceScheduleSlotDto>();

            if (schedule.RecurrenceType == RecurrenceType.Once)
            {
                if (schedule.StartDate.Date >= startDate.Date && schedule.StartDate.Date <= endDate.Date)
                {
                    foreach (var timeSlot in schedule.TimeSlots)
                    {
                        slots.Add(new ExperienceScheduleSlotDto
                        {
                            ScheduleId = schedule.Id,
                            Date = schedule.StartDate.Date,
                            StartTime = timeSlot.StartTime,
                            EndTime = timeSlot.EndTime
                        });
                    }
                }
            }
            if (schedule.RecurrenceType == RecurrenceType.Weekly)
            {
                var currentDate = startDate.Date > schedule.StartDate.Date
                    ? startDate.Date
                    : schedule.StartDate.Date;

                var lastDate = endDate.Date < (schedule.EndDate?.Date ?? DateTime.MaxValue)
                    ? endDate.Date
                    : (schedule.EndDate?.Date ?? DateTime.MaxValue);

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
