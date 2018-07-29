using Autofac;
using EventBus.Kafka;
using System.Reflection;
using MicroservicesPlayground.EventBus.Abstractions;
using Ordering.SignalrHub.IntegrationEvents.Events;

namespace Ordering.SignalrHub.AutofacModules
{
    public class ApplicationModule
        : Autofac.Module
    {

        public string QueriesConnectionString { get; }

        public ApplicationModule()
        {
        }

        protected override void Load(ContainerBuilder builder)
        {
            KafkaServicesRegistration.Register(builder);
            builder.RegisterAssemblyTypes(typeof(ScheduleConfirmedEvent).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));


        }
    }
}
