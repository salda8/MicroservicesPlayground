using System.Collections.Generic;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;


namespace EventBus.Kafka {
    public class KafkaConsumerFactory : IKafkaConsumerFactory {
        public Consumer<TKey, TValue> CreateConsumer<TKey, TValue>(Dictionary<string, object> configuration, IDeserializer<TKey> keyDeserializer, IDeserializer<TValue> valueDeserializer)where TKey : class where TValue : class {
            return new Consumer<TKey, TValue>(configuration, keyDeserializer, valueDeserializer);
        }

        public Consumer<Ignore, string> CreateConsumer(Dictionary<string, object> configuration, IDeserializer<string> valueDeserializer) {
            
            return new Consumer<Ignore, string>(configuration, null, valueDeserializer);
        }
    }

    public interface IKafkaConsumerFactory {
        Consumer<TKey, TValue> CreateConsumer<TKey, TValue>(Dictionary<string, object> configuration, IDeserializer<TKey> keyDeserializer, IDeserializer<TValue> valueDeserializer)where TKey : class where TValue : class;
        Consumer<Ignore, string> CreateConsumer(Dictionary<string, object> configuration, IDeserializer<string> valueDeserializer);

      
      
    }

}