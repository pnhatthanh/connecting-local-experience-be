using BuildingBlocks.Application.CQRS.Query;
using Booking.Application.Dtos;
using Booking.Domain.Repositories;
using Booking.Domain.Specifications;
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
            var spec = new BookingByUserIdSpecification(request.UserId);
            var bookings = await _bookingRepository.GetAllAsync(spec, b => b.Payment!, b => b.Cancellation!);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }
    }
}
