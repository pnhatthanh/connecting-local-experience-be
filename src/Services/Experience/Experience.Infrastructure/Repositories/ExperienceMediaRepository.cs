using BuildingBlocks.EntityFramework;
using Experience.Domain.Entities;
using Experience.Domain.Repositories;
using Experience.Infrastructure.Data;

namespace Experience.Infrastructure.Repositories
{
    public class ExperienceMediaRepository(ExperienceDbContext context) 
        : BaseRepository<ExperienceMediaEntity>(context), IExperienceMediaRepository
    {
    }
}
