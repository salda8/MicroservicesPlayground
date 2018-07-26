using Autofac;
using EventBus.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ordering.SignalrHub.IntegrationEvents.Events;
using EventBus.Abstractions;

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
