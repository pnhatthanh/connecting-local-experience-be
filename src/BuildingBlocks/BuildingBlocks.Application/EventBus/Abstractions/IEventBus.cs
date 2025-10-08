using BuildingBlocks.Application.EventBus.Events;

namespace BuildingBlocks.Application.EventBus.Abstractions
{
    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : IntegrationEvent;

        Task SubscribeAsync<TEvent, THandler>(CancellationToken cancellationToken = default)
            where TEvent : IntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>;

        void Unsubscribe<TEvent, THandler>()
            where TEvent : IntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>;
    }
}