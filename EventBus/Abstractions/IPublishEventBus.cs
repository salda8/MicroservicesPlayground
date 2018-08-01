using MicroservicesPlayground.EventBus.Events;

namespace MicroservicesPlayground.EventBus.Abstractions
{
    public interface IPublishEventBus
    {
         void Publish(IntegrationEvent @event);
    }
}
