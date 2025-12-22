using BuildingBlocks.Application.EventBus.Abstractions;
using BuildingBlocks.Domain.Interfaces;
using User.Application.Events;
using User.Domain.Repositories;
using User.Domain.Specifications;

namespace User.Application.EventHandlers;

public class UserRatedExperienceEventHandler : IIntegrationEventHandler<UserRatedExperienceEvent>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    public UserRatedExperienceEventHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task HandleAsync(UserRatedExperienceEvent @event, CancellationToken cancellationToken = default)
    {
        var specification = new UserByIdSpecification(@event.HostId);
        var host = await _userRepository.GetBySpecAsync(specification, u => u.HostProfile!);
        if (host != null && host.HostProfile != null)
        {
            host.HostProfile.TotalReviews ++;
            host.HostProfile.RatingAvg = ((host.HostProfile.RatingAvg * (host.HostProfile.TotalReviews - 1)) + @event.Rating) / host.HostProfile.TotalReviews;
            _userRepository.Update(host);
            await _unitOfWork.SaveChangeAsync();
        }
    }
}