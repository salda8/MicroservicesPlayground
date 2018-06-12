using Confluent.Kafka;
using System;
using System.Collections.Generic;

namespace EventBusKafka
{
    public interface IKafkaConnection : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IDictionary<string, object> Configuration();

    }

    public interface IKafkaConsumerConnection : IDisposable
    {
        Consumer CreateConsumer();

    }

    public interface IKafkaProducerConnection : IDisposable
    {
        Producer CreateProducer();

    }
}