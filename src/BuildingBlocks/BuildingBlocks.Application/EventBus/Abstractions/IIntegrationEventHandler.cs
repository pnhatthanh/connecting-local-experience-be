using BuildingBlocks.Application.EventBus.Events;

namespace BuildingBlocks.Application.EventBus.Abstractions
{
    public interface IIntegrationEventHandler<in TEvent>
     where TEvent : IntegrationEvent
    {
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
    }
}