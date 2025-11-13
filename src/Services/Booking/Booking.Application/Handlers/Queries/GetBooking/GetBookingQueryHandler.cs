using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Domain.Exceptions;
using Booking.Application.Dtos;
using Booking.Domain.Repositories;
using MapsterMapper;

namespace Booking.Application.Handlers.Queries.GetBooking
{
    public class GetBookingQueryHandler : IQueryHandler<GetBookingQuery, BookingDto?>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public GetBookingQueryHandler(IBookingRepository bookingRepository, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        public async Task<BookingDto?> Handle(GetBookingQuery request, CancellationToken cancellationToken)
        {
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId);
            return booking == null ? null : _mapper.Map<BookingDto>(booking);
        }
    }
}
