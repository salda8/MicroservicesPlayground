using EventBus.Events;

namespace EventBus.Abstractions
{
    public interface ISubscriptionEventBus
    {

        void Subscribe<T, TH>(string eventName)
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void SubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        void UnsubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        void Unsubscribe<T, TH>(string eventName)
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent;
    }
}
