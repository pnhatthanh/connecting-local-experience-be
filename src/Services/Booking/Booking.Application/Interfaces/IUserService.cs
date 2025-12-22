using Booking.Application.Dtos;

namespace Booking.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetUserAsync(Guid userId);
        Task<List<HostProfileDto>> GetHostProfilesAsync(List<Guid> hostIds, CancellationToken cancellationToken = default);
    }
}
