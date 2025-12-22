using Booking.Application.Events;
using BuildingBlocks.Application.EventBus.Abstractions;
using BuildingBlocks.Domain.Interfaces;
using User.Domain.Repositories;
using User.Domain.Specifications;

namespace User.Application.EventHandlers;

public class BookingConfirmedEventHandler : IIntegrationEventHandler<BookingConfirmedEvent>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    public BookingConfirmedEventHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task HandleAsync(BookingConfirmedEvent @event, CancellationToken cancellationToken = default)
    {
        var specification = new UserByIdSpecification(@event.HostId);
        var host = await _userRepository.GetBySpecAsync(specification, u => u.HostProfile!);
        if (host != null && host.HostProfile != null)
        {
            host.HostProfile.TotalBookings ++;
            _userRepository.Update(host);
            await _unitOfWork.SaveChangeAsync();
        }
    }
}