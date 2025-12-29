using BuildingBlocks.EntityFramework;
using Experience.Domain.Entities;
using Experience.Domain.Enums;
using Experience.Domain.Repositories;
using Experience.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Experience.Infrastructure.Repositories
{
    public class ExperienceRepository(ExperienceDbContext context) 
        : BaseRepository<ExperienceEntity>(context), IExperienceRepository
    {
        public async Task<List<ExperienceEntity>> GetExperiencesByIdsAsync(List<Guid> experienceIds)
        {
            return await _dbSet.Where(e => experienceIds.Contains(e.Id) && e.Status == ExperienceStatus.Approved)
                .Include(e => e.Media)
                .Include(e => e.Category)
                .ToListAsync();
        }
    }
}
