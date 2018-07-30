using Autofac;

namespace EventBus.Kafka
{
    public static class KafkaServicesRegistration
    {
        public static void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<KafkaConsumerFactory>().As<IKafkaConsumerFactory>().SingleInstance();


        }
    }
}
