using BuildingBlocks.EntityFramework;
using Experience.Domain.Entities;
using Experience.Domain.Enums;
using Experience.Domain.Repositories;
using Experience.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Experience.Infrastructure.Repositories
{
    public class ExperienceRepository : BaseRepository<ExperienceEntity>, IExperienceRepository
    {
        public ExperienceRepository(ExperienceDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ExperienceEntity>> GetByHostIdAsync(Guid hostId)
        {
            return await _dbSet
                .Where(e => e.HostId == hostId)
                .Include(e => e.Media)
                .Include(e => e.Schedules)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExperienceEntity>> GetApprovedExperiencesAsync()
        {
            return await _dbSet
                .Where(e => e.Status == ExperienceStatus.Approved)
                .Include(e => e.Media)
                .Include(e => e.Schedules)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExperienceEntity>> GetNearbyExperiencesAsync(Point userLocation, double radiusInMeters)
        {
            return await _dbSet
                .Where(e => e.Status == ExperienceStatus.Approved && 
                           e.Location.IsWithinDistance(userLocation, radiusInMeters))
                .OrderBy(e => e.Location.Distance(userLocation))
                .Include(e => e.Media)
                .Include(e => e.Schedules)
                .ToListAsync();
        }
    }
}
