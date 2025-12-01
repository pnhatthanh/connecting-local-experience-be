namespace Booking.Application.Interfaces
{
    public interface IUserService
    {
        Task<Dtos.UserDto?> GetUserAsync(Guid userId);
    }
}
