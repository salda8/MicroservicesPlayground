using EventBus.Events;
using System;

namespace EventBus.Abstractions
{
    public interface IEventBus : ISubscriptionEventBus,IPublishEventBus
    {
             
    }

    public interface IPublishEventBus
    {
         void Publish(IntegrationEvent @event);
    }
}
