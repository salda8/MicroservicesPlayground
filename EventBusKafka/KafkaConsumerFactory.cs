using Confluent.Kafka;
using System.Collections.Generic;

namespace EventBus.Kafka
{
    public class KafkaConsumerFactory : IKafkaConsumerFactory
    {
        public Consumer<TKey, TValue> CreateConsumer<TKey, TValue>(IEnumerable<KeyValuePair<string, string>> configuration) where TKey : class where TValue : class => new Consumer<TKey, TValue>(configuration);

        public Consumer<Ignore, string> CreateConsumer(IEnumerable<KeyValuePair<string, string>> configuration) => new Consumer<Ignore, string>(configuration);
    }

    public interface IKafkaConsumerFactory
    {
        Consumer<TKey, TValue> CreateConsumer<TKey, TValue>(IEnumerable<KeyValuePair<string, string>> configuration) where TKey : class where TValue : class;

        Consumer<Ignore, string> CreateConsumer(IEnumerable<KeyValuePair<string, string>> configuration);
    }
}