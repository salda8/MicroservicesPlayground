using EventBusKafka.Abstractions;
using EventBusKafka.Events;

namespace EventBusKafka
{
    internal class EventBusKafka : IEventBus
    {
        private readonly IKafkaProducerConnection producerConnection;
        private readonly IKafkaConsumerConnection consumerConnection;
        private readonly IEventBusSubscriptionsManager subscriptionsManager;

        public EventBusKafka(IKafkaConsumerConnection consumerConnection, IKafkaProducerConnection producerConnection, IEventBusSubscriptionsManager subscriptionsManager)
        {
            this.subscriptionsManager = subscriptionsManager;
            subscriptionsManager.OnEventRemoved += SubscriptionManager_OnEventRemoved;
            this.consumerConnection = consumerConnection;
            this.producerConnection = producerConnection;
        }

        private void SubscriptionManager_OnEventRemoved(object sender, string e)
        {
            ConnectIfNotConnected();
        }

        private void ConnectIfNotConnected()
        {
            
        }

        public void Publish(IntegrationEvent @event)
        {
            using (var producer = producerConnection.CreateProducer())
            {
                var eventName = @event.GetType()
                   .Name;
                //var message = JsonConvert.SerializeObject(@event);
                //var body = Encoding.UTF8.GetBytes(message);
               // producer.ProduceAsync(eventName, null, @event.);

            }
            ;
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {

        }

        public void SubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
        }

        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
        }

        public void UnsubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
        }
    }
}