using BuildingBlocks.Domain.Interfaces;
using Experience.Domain.Entities;
using NetTopologySuite.Geometries;

namespace Experience.Domain.Repositories
{
    public interface IExperienceRepository : IBaseRepository<ExperienceEntity>
    {
        Task<IEnumerable<ExperienceEntity>> GetByHostIdAsync(Guid hostId);
        Task<IEnumerable<ExperienceEntity>> GetApprovedExperiencesAsync();
        Task<IEnumerable<ExperienceEntity>> GetNearbyExperiencesAsync(Point userLocation, double radiusInMeters);
    }
}
