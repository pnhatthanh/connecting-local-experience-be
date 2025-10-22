using BuildingBlocks.Domain.Interfaces;
using Experience.Domain.Entities;

namespace Experience.Domain.Repositories
{
    public interface IExperienceItineraryRepository : IBaseRepository<ExperienceItineraryEntity>
    {
        Task<IEnumerable<ExperienceItineraryEntity>> GetByExperienceIdAsync(Guid experienceId);
    }
}
