using BuildingBlocks.Application.CQRS.Query;
using Booking.Application.Dtos;
using Booking.Domain.Enums;
using Booking.Domain.Repositories;
using Booking.Domain.Specifications;
using MapsterMapper;

namespace Booking.Application.Handlers.Queries.GetExperienceBookings
{
    public class GetExperienceBookingsQueryHandler : IQueryHandler<GetExperienceBookingsQuery, List<BookingDto>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public GetExperienceBookingsQueryHandler(
            IBookingRepository bookingRepository,
            IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        public async Task<List<BookingDto>> Handle(GetExperienceBookingsQuery request, CancellationToken cancellationToken)
        {
            var spec = new BookingByExperienceIdSpecification(request.ExperienceId);
            var bookings = await _bookingRepository.GetAllAsync(
                spec, 
                b => b.Payment!, 
                b => b.Cancellation!
            );
            var filteredBookings = bookings
                .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending)
                .AsQueryable();

            if (request.Date.HasValue)
            {
                filteredBookings = filteredBookings.Where(b => b.Date == request.Date.Value);
            }

            if (request.StartTime.HasValue)
            {
                filteredBookings = filteredBookings.Where(b => b.StartTime == request.StartTime.Value);
            }

            var bookingList = filteredBookings.ToList();
            var bookingDtos = _mapper.Map<List<BookingDto>>(bookingList);

            return bookingDtos;
        }
    }
}
