using BuildingBlocks.EntityFramework;
using Experience.Domain.Entities;
using Experience.Domain.Repositories;
using Experience.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Experience.Infrastructure.Repositories
{
    public class ExperienceScheduleSlotRepository(ExperienceDbContext context) 
        : BaseRepository<ExperienceScheduleSlotEntity>(context), IExperienceScheduleSlotRepository
    {
        public async Task<ExperienceScheduleSlotEntity?> GetSlotByDateAndTimeAsync(
            Guid scheduleId, DateOnly date, TimeSpan startTime, TimeSpan endTime)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.ScheduleId == scheduleId && 
                    s.Date == date && 
                    s.StartTime == startTime && 
                    s.EndTime == endTime);
        }
    }
}
