using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Newtonsoft.Json;

namespace PaymentApi
{
    public class KafkaClient
    {
        public KafkaClient()
        {

        }

        public void Poll(string topicName)
        {
            var config = new Dictionary<string, object>
            {
                { "group.id", "simple-csharp-consumer" },
                { "bootstrap.servers", "127.0.0.1:9092" },
                //{ "schema.registry.url", "127.0.0.1:8081" },
            };

            using (var consumer = new Consumer<Ignore, string>(config, null, new StringDeserializer(Encoding.UTF8)))
            {
                consumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(topicName, 0, 0) });

                // Raised on critical errors, e.g. connection failures or all brokers down.
                consumer.OnError += (_, error)
                    => Console.WriteLine($"Error: {error}");

                // Raised on deserialization errors or when a consumed message has an error != NoError.
                consumer.OnConsumeError += (_, error)
                    => Console.WriteLine($"Consume error: {error}");

                while (true)
                {
                    //Message<Ignore, string> msg;

                    if (consumer.Consume(out ConsumerRecord<Ignore, string> msg, TimeSpan.FromSeconds(1)))
                    {
                        var serObject = JsonConvert.DeserializeObject(msg.Message.Value);


                        Console.WriteLine($"Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {msg.Value}");
                    }
                }
            }
        }
    }
}
