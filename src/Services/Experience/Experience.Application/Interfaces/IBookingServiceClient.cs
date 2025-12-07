namespace Experience.Application.Interfaces
{
    public interface IBookingServiceClient
    {
        Task<bool> HasUserBookedExperienceAsync(Guid userId, Guid experienceId);
    }
}
