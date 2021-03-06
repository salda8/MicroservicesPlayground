using MicroservicesPlayground.EventBus.Events;

namespace MicroservicesPlayground.EventBus.Abstractions
{
    public interface ISubscriptionEventBus
    {

        void Subscribe<T, TH>(string eventName=null)
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void SubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        void UnsubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        void Unsubscribe<T, TH>(string eventName=null)
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent;
    }
}
