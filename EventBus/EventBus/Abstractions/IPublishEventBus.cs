using EventBus.Events;

namespace EventBus.Abstractions
{
    public interface IPublishEventBus
    {
         void Publish(IntegrationEvent @event);
    }
}
