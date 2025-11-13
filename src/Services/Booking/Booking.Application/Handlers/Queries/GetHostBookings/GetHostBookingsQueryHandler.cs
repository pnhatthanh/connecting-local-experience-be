using BuildingBlocks.Application.CQRS.Query;
using Booking.Application.Dtos;
using Booking.Domain.Repositories;
using MapsterMapper;

namespace Booking.Application.Handlers.Queries.GetHostBookings
{
    public class GetHostBookingsQueryHandler : IQueryHandler<GetHostBookingsQuery, IEnumerable<BookingDto>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public GetHostBookingsQueryHandler(IBookingRepository bookingRepository, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookingDto>> Handle(GetHostBookingsQuery request, CancellationToken cancellationToken)
        {
            var bookings = await _bookingRepository.GetByHostIdAsync(request.HostId);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }
    }
}
