using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using Booking.Domain.Enums;
using Booking.Domain.Repositories;
using BuildingBlocks.Application.Interfaces;

namespace Booking.Application.Handlers.Commands.ToggleBookingStatus
{
    public class ToggleBookingStatusCommandHandler : ICommandHandler<ToggleBookingStatusCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public ToggleBookingStatusCommandHandler( IBookingRepository bookingRepository, ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(ToggleBookingStatusCommand request, CancellationToken cancellationToken)
        {
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId)
                ?? throw new NotFoundException("Booking not found");
            if (booking.HostId != _currentUserService.UserId)
                throw new ForbiddenException("You do not have permission to toggle the status of this booking");
            var status = Enum.Parse<BookingStatus>(request.Status, true);
            if (booking.Status != BookingStatus.Confirmed && booking.Status != BookingStatus.Completed)
                throw new BadRequestException("Only bookings with Confirmed or Completed status can be toggled");
            if (booking.Status == status)
                throw new BadRequestException($"Booking is already in {status} status");
            booking.Status = status;
            booking.UpdatedAt = DateTime.UtcNow;
            _bookingRepository.Update(booking);
            await _unitOfWork.SaveChangeAsync();

            return true;
        }
    }
}
