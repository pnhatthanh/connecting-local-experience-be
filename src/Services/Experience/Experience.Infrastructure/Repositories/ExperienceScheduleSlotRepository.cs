using BuildingBlocks.EntityFramework;
using Experience.Domain.Entities;
using Experience.Domain.Enums;
using Experience.Domain.Repositories;
using Experience.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Experience.Infrastructure.Repositories
{
    public class ExperienceScheduleSlotRepository : BaseRepository<ExperienceScheduleSlotEntity>, IExperienceScheduleSlotRepository
    {
        public ExperienceScheduleSlotRepository(ExperienceDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ExperienceScheduleSlotEntity>> GetByScheduleIdAsync(Guid scheduleId)
        {
            return await _dbSet
                .Where(s => s.ScheduleId == scheduleId)
                .OrderBy(s => s.Date)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExperienceScheduleSlotEntity>> GetAvailableSlotsAsync(Guid experienceId, DateTime fromDate, DateTime toDate)
        {
            return await _dbSet
                .Include(s => s.Schedule)
                .Where(s => s.Schedule.ExperienceId == experienceId 
                    && s.Date >= fromDate 
                    && s.Date <= toDate
                    && s.Status == SlotStatus.Open
                    && s.AvailableSlots > 0)
                .OrderBy(s => s.Date)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }
    }
}
