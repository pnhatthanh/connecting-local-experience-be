using BuildingBlocks.Domain.Interfaces;
using Experience.Domain.Entities;

namespace Experience.Domain.Repositories
{
    public interface IExperienceScheduleSlotRepository : IBaseRepository<ExperienceScheduleSlotEntity>
    {
        Task<IEnumerable<ExperienceScheduleSlotEntity>> GetByScheduleIdAsync(Guid scheduleId);
        Task<IEnumerable<ExperienceScheduleSlotEntity>> GetAvailableSlotsAsync(Guid experienceId, DateTime fromDate, DateTime toDate);
    }
}
