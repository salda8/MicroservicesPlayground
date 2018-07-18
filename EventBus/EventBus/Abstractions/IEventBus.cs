using System;

namespace EventBus.Abstractions
{
    public interface IEventBus : ISubscriptionEventBus,IPublishEventBus
    {
             
    }
}
