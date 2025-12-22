using BuildingBlocks.Domain.Interfaces;
using Experience.Domain.Entities;

namespace Experience.Domain.Repositories
{
    public interface IExperienceRepository : IBaseRepository<ExperienceEntity>
    {
        public Task<List<ExperienceEntity>> GetExperiencesByIdsAsync(List<Guid> experienceIds);
    }
}
