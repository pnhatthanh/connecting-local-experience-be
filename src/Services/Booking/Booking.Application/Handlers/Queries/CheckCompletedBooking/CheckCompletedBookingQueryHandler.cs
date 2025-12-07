using BuildingBlocks.Application.CQRS.Query;
using Booking.Domain.Repositories;
using Booking.Domain.Specifications;

namespace Booking.Application.Handlers.Queries.CheckCompletedBooking
{
    public class CheckCompletedBookingQueryHandler : IQueryHandler<CheckCompletedBookingQuery, bool>
    {
        private readonly IBookingRepository _bookingRepository;

        public CheckCompletedBookingQueryHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<bool> Handle(CheckCompletedBookingQuery request, CancellationToken cancellationToken)
        {
            var specification = new BookingByUserAndExperienceSpecification(request.UserId, request.ExperienceId);
            var isExisted = await _bookingRepository.CheckExistsAsync(specification);
            return isExisted;
        }
    }
}
