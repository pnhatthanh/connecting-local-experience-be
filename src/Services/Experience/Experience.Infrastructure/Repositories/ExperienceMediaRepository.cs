using BuildingBlocks.EntityFramework;
using Experience.Domain.Entities;
using Experience.Domain.Repositories;
using Experience.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Experience.Infrastructure.Repositories
{
    public class ExperienceMediaRepository : BaseRepository<ExperienceMediaEntity>, IExperienceMediaRepository
    {
        public ExperienceMediaRepository(ExperienceDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ExperienceMediaEntity>> GetByExperienceIdAsync(Guid experienceId)
        {
            return await _dbSet
                .Where(m => m.ExperienceId == experienceId)
                .OrderBy(m => m.Order)
                .ToListAsync();
        }
    }
}
