using BuildingBlocks.Application.CQRS.Query;
using Booking.Application.Dtos;
using Booking.Domain.Repositories;
using Booking.Domain.Specifications;
using MapsterMapper;
using BuildingBlocks.Application.Interfaces;

namespace Booking.Application.Handlers.Queries.GetHostBookings
{
    public class GetHostBookingsQueryHandler : IQueryHandler<GetHostBookingsQuery, IEnumerable<BookingDto>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetHostBookingsQueryHandler(IBookingRepository bookingRepository, IMapper mapper, 
            ICurrentUserService currentUserService)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<BookingDto>> Handle(GetHostBookingsQuery request, CancellationToken cancellationToken)
        {
            var hostId = _currentUserService.UserId;
            var spec = new BookingByHostIdSpecification(hostId, request.Date, request.StartTime);
            var bookings = await _bookingRepository.GetAllAsync(spec, b => b.Payment!, b => b.Cancellation!);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }
    }
}
