using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using EventBus.Abstractions;
using EventBus.Events;
using EventFlow.Kafka;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EventBus.Kafka
{
    //    1. One Consumer Per Thread
    //A simple option is to give each thread its own consumer instance.Here are the pros and cons of this approach:
    //PRO: It is the easiest to implement
    //PRO: It is often the fastest as no inter-thread co-ordination is needed
    //PRO: It makes in-order processing on a per-partition basis very easy to implement (each thread just processes messages in the order it receives them).
    //CON: More consumers means more TCP connections to the cluster(one per thread). In general Kafka handles connections very efficiently so this is generally a small cost.
    //CON: Multiple consumers means more requests being sent to the server and slightly less batching of data which can cause some drop in I/O throughput.
    //CON: The number of total threads across all processes will be limited by the total number of partitions.
    //2. Decouple Consumption and Processing
    //Another alternative is to have one or more consumer threads that do all data consumption and hands off ConsumerRecords instances to a blocking queue consumed by a pool of processor 
    // threads that actually handle the record processing.This option likewise has pros and cons:
    //PRO: This option allows independently scaling the number of consumers and processors.This makes it possible to have a single consumer that feeds many processor threads, 
    //avoiding any limitation on partitions.
    //CON: Guaranteeing order across the processors requires particular care as the threads will execute independently an earlier chunk of data may actually be processed after a later chunk of data just due to the luck of thread execution timing.For processing that has no ordering requirements this is not a problem.
    //CON: Manually committing the position becomes harder as it requires that all threads co-ordinate to ensure that processing is complete for that partition.
    //There are many possible variations on this approach.For example each processor thread can have its own queue, and the consumer threads can hash into these queues using the TopicPartition to ensure in-order consumption and simplify commit.
    public class EventBusKafka : ISubscriptionEventBus
    {
        private readonly IDeserializer<string> valueDeserializer;
        private readonly IKafkaConsumerConfiguration configuration;
        private readonly ILifetimeScope autofac;
        private readonly IEventBusSubscriptionsManager subscriptionsManager;
        private const string AUTOFAC_SCOPE_NAME = "scope";
        private List<string> subscribedTopics = new List<string> { "eventflow.domainevent.appointment-aggregate" };

        public IEnumerable<string> SubscribedTopics => subscribedTopics;

        public IKafkaConsumerFactory consumerFactory;
        public Dictionary<string, object> Configuration { get; }

        private CancellationToken cancellationToken = new CancellationToken();

        public EventBusKafka(IEventBusSubscriptionsManager subscriptionsManager, IKafkaConsumerFactory consumerFactory, ILifetimeScope autofac, IKafkaConsumerConfiguration configuration, IDeserializer<string> valueDeserializer)
        {
            this.subscriptionsManager = subscriptionsManager ?? new InMemoryEventBusSubscriptionsManager();
            this.consumerFactory = consumerFactory;
            this.autofac = autofac;
            this.configuration = configuration;
            this.valueDeserializer = valueDeserializer;
            this.subscribedTopics = configuration.SubscribedTopics;
            Configuration = configuration.Configuration;
            valueDeserializer.Configure(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(StringDeserializer.KeyEncodingConfigParam, configuration.Encoding.HeaderName) }, true);
            CreateOneConsumerPerTopic();
        }



        public void SubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {

            subscriptionsManager.AddDynamicSubscription<TH>(eventName);
        }

        public void Subscribe<T, TH>(string eventName = null)
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {

            subscriptionsManager.AddSubscription<T, TH>(subscriptionsManager.GetEventName<T>(eventName));
        }


        public void Unsubscribe<T, TH>(string eventName = null)
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent
        {
            subscriptionsManager.RemoveSubscription<T, TH>(subscriptionsManager.GetEventName<T>(eventName));
        }

        public void UnsubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            subscriptionsManager.RemoveDynamicSubscription<TH>(eventName);
        }

        public void AddTopicToSubscribeTo(string topicName, CancellationToken cancelToken = default(CancellationToken))
        {
            subscribedTopics.Add(topicName);
            Task.Factory.StartNew(async () => await ConsumerAction(topicName, cancelToken), cancelToken == default(CancellationToken) ? cancellationToken : cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

        }

        private async Task Consume(Consumer<Ignore, string> consumer, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (consumer.Consume(out ConsumerRecord<Ignore, string> record, TimeSpan.FromSeconds(1)))
                {
                    var serObject = JsonConvert.DeserializeObject<KafkaEvent>(record.Message.Value);
                    await ProcessEventAsync(serObject.Metadata.EventName, record.Message.Value);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }

        }

        private void CreateOneConsumerPerTopic()
        {

            foreach (string topic in subscribedTopics)
            {

                Task.Factory.StartNew(async () => await ConsumerAction(topic, cancellationToken), cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            }

        }
        async Task ConsumerAction(string topic, CancellationToken cancellationToken)
        {
            using (var consumer = consumerFactory.CreateConsumer(Configuration, valueDeserializer))
            {
                consumer.Subscribe(topic);
                await Consume(consumer, cancellationToken);
            }
        }

        private async Task ProcessEventAsync(string eventName, string message)
        {
            if (subscriptionsManager.HasSubscriptionsForEvent(eventName))
            {
                using (var scope = autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME))
                {
                    foreach (var subscription in subscriptionsManager.GetHandlersForEvent(eventName))
                    {
                        if (subscription.IsDynamic)
                        {
                            var handler = scope.ResolveOptional(subscription.HandlerType) as IDynamicIntegrationEventHandler;
                            dynamic eventData = JObject.Parse(message);
                            await handler.Handle(eventData).ConfigureAwait(false);
                        }
                        else
                        {
                            var eventType = subscriptionsManager.GetEventTypeByName(eventName);
                            var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                            var handler = scope.ResolveOptional(subscription.HandlerType);
                            var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                            await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                        }
                    }
                }
            }
        }


    }
}