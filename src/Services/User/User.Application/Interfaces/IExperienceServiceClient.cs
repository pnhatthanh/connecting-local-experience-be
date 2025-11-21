using User.Application.DTOs;

namespace User.Application.Interfaces
{
    public interface IExperienceServiceClient
    {
        Task<List<ExperienceSummaryDto>> GetExperiencesByIdsAsync(List<Guid> experienceIds);
        Task<bool> CheckExperienceExistsAsync(Guid experienceId);
    }
}
