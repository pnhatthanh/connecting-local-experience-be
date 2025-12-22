using BuildingBlocks.Domain.Interfaces;
using Experience.Domain.Entities;

namespace Experience.Domain.Repositories
{
    public interface IExperienceScheduleSlotRepository : IBaseRepository<ExperienceScheduleSlotEntity>
    {
        Task<ExperienceScheduleSlotEntity?> GetSlotByDateAndTimeAsync(Guid scheduleId, DateOnly date, TimeSpan startTime, TimeSpan endTime);
    }
}
