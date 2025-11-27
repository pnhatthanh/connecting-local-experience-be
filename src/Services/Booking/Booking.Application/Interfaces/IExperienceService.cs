using Booking.Application.Dtos;

namespace Booking.Application.Interfaces
{
    public interface IExperienceService
    {
        Task<ExperienceDto?> GetExperienceAsync(Guid experienceId);
        Task<bool> ValidateAvailabilityAsync(Guid experienceId, DateOnly date, TimeSpan startTime, TimeSpan endTime, int adults, int children);
    }
}
