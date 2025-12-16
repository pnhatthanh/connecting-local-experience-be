using Booking.Application.Dtos;
using Booking.Domain.Enums;
using Booking.Domain.Repositories;
using Booking.Domain.Specifications;
using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Application.Dtos;
using MapsterMapper;

namespace Booking.Application.Handlers.Queries.GetRecentBookings
{
    public class GetRecentBookingsQueryHandler : IQueryHandler<GetRecentBookingsQuery, PaginationResult<BookingDto>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;
        public GetRecentBookingsQueryHandler(
            IBookingRepository bookingRepository,
            IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }
        
        public async Task<PaginationResult<BookingDto>> Handle(GetRecentBookingsQuery request, CancellationToken cancellationToken)
        {
            var specification = new BookingByStatusSpecification(BookingStatus.Pending);
            var bookings = await _bookingRepository.GetPagedListAsync(
                specification, 
                request.PageNumber,
                request.PageSize,
                sortBy: "CreatedAt",
                isAscending: false);
            var totalCount = await _bookingRepository.CountAsync(specification);
            return new PaginationResult<BookingDto>
            {
                Data = _mapper.Map<List<BookingDto>>(bookings),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}