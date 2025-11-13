using BuildingBlocks.Application.CQRS.Query;
using Booking.Application.Dtos;
using Booking.Domain.Repositories;
using MapsterMapper;

namespace Booking.Application.Handlers.Queries.GetUserBookings
{
    public class GetUserBookingsQueryHandler : IQueryHandler<GetUserBookingsQuery, IEnumerable<BookingDto>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public GetUserBookingsQueryHandler(IBookingRepository bookingRepository, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookingDto>> Handle(GetUserBookingsQuery request, CancellationToken cancellationToken)
        {
            var bookings = await _bookingRepository.GetByUserIdAsync(request.UserId);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }
    }
}
