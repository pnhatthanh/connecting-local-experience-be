using Experience.Application.Dtos;

namespace Experience.Application.Interfaces
{
    public interface IUserServiceClient
    {
        Task<List<Guid>> CheckExperiencesInWishlistAsync(Guid userId, List<Guid> experienceIds);
        Task<Dictionary<Guid, UserInfoDto>> GetUsersInfoAsync(List<Guid> userIds);
    }
}
