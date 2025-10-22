using BuildingBlocks.EntityFramework;
using Experience.Domain.Entities;
using Experience.Domain.Repositories;
using Experience.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Experience.Infrastructure.Repositories
{
    public class ExperienceScheduleRepository : BaseRepository<ExperienceScheduleEntity>, IExperienceScheduleRepository
    {
        public ExperienceScheduleRepository(ExperienceDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ExperienceScheduleEntity>> GetByExperienceIdAsync(Guid experienceId)
        {
            return await _dbSet
                .Where(s => s.ExperienceId == experienceId)
                .Include(s => s.Slots)
                .ToListAsync();
        }
    }
}
