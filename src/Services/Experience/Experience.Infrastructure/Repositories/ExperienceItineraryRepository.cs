using BuildingBlocks.EntityFramework;
using Experience.Domain.Entities;
using Experience.Domain.Repositories;
using Experience.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Experience.Infrastructure.Repositories
{
    public class ExperienceItineraryRepository : BaseRepository<ExperienceItineraryEntity>, IExperienceItineraryRepository
    {
        public ExperienceItineraryRepository(ExperienceDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ExperienceItineraryEntity>> GetByExperienceIdAsync(Guid experienceId)
        {
            return await _dbSet
                .Where(i => i.ExperienceId == experienceId)
                .OrderBy(i => i.StepNumber)
                .ToListAsync();
        }
    }
}
