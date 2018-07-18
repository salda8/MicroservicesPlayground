using System;
using Autofac;
using EventFlow.Kafka;

namespace EventBus.Kafka
{
    public static class ServicesRegistration
    {
        public static void Register(ContainerBuilder containerBuilder){
            containerBuilder.RegisterType<KafkaConsumerFactory>().As<IKafkaConsumerFactory>().SingleInstance();
            containerBuilder.RegisterType<EventBus.Kafka.KafkaConsumerConfiguration>().As<IKafkaConsumerConfiguration>();

        }
    }
}
