using BuildingBlocks.Domain.Interfaces;
using Booking.Domain.Entities;

namespace Booking.Domain.Repositories
{
    public interface IBookingRepository : IBaseRepository<BookingEntity>
    {
        Task<BookingEntity?> GetByBookingCodeAsync(string bookingCode);
        Task<IEnumerable<BookingEntity>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<BookingEntity>> GetByHostIdAsync(Guid hostId);
        Task<IEnumerable<BookingEntity>> GetByExperienceIdAsync(Guid experienceId);
        Task<string> GenerateBookingCodeAsync();
    }
}
