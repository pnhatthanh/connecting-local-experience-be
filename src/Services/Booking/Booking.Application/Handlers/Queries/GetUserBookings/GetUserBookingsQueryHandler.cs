using BuildingBlocks.Application.CQRS.Query;
using Booking.Application.Dtos;
using Booking.Domain.Repositories;
using Booking.Domain.Specifications;
using MapsterMapper;
using BuildingBlocks.Application.Interfaces;

namespace Booking.Application.Handlers.Queries.GetUserBookings
{
    public class GetUserBookingsQueryHandler : IQueryHandler<GetUserBookingsQuery, IEnumerable<BookingSummaryDto>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetUserBookingsQueryHandler(IBookingRepository bookingRepository, ICurrentUserService currentUserService, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookingSummaryDto>> Handle(GetUserBookingsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var spec = new BookingByUserIdSpecification(userId);
            var bookings = await _bookingRepository.GetAllAsync(spec);
            var sortedBookings = bookings.OrderByDescending(b => b.CreatedAt).ToList();
            return _mapper.Map<IEnumerable<BookingSummaryDto>>(sortedBookings);
        }
    }
}
