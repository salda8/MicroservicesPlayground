using AppointmentApi;
using AppointmentApi.Models.Appointment.Integration;
using AppointmentApi.MongoDb;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBus.Kafka;
using EventFlow.Extensions;
using EventFlow.Logs;
using EventFlow.MongoDB.ReadStores;
using EventFlow.ReadStores;
using EventFlow.Snapshots.Strategies;
using MicroservicesPlayground.EventBus;
using MicroservicesPlayground.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Payments.Application;
using Payments.Domain.Payments.Providers;
using Payments.Domain.Payments.Providers.Types;
using SchedulingApi.Controllers;

namespace SchedulingApi
{
    public class AutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // The generic ILogger<TCategoryName> service was added to the ServiceCollection by ASP.NET Core.
            // It was then registered with Autofac using the Populate method in ConfigureServices.
            builder.RegisterType<SerilogLogger>().As<ILog>();
            builder.RegisterType<ExceptionHandlingMiddleware>();
            builder.RegisterType<MongoDatabaseFactory>().As<IMongoDatabaseFactory>();
            builder.RegisterType<AppointmentService>().As<IAppointmentService>();
            builder.RegisterType<InMemoryEventBusSubscriptionsManager>().As<IEventBusSubscriptionsManager>();
            builder.RegisterType<IntegrationTestEventHandler>().As<IIntegrationEventHandler>();
            // builder.RegisterType<SagaUpdater<AppointmentAggregate, AppointmentId, AppointmentBookedEvent, AppointmentSaga>>().As<ISagaUpdater<AppointmentAggregate, AppointmentId, AppointmentBookedEvent, AppointmentSaga>>();
            builder.RegisterType<OrdersApplicationService>().As<IOrdersApplicationService>();
            builder.RegisterType<PaymentsApplicationService>().As<IPaymentsApplicationService>();
            builder.RegisterType<PaymentProviderFactory>().As<IPaymentProviderFactory>();
            builder.RegisterType<Payments.Domain.Payments.Providers.ConfigurationProvider>().As<Payments.Domain.Payments.Providers.IConfigurationProvider>();
            builder.RegisterType<TestProvider1PaymentProvider>().As<IPaymentProvider>();
            builder.RegisterType<ReadModelFactory<AppointmentReadModel>>().As<IReadModelFactory<AppointmentReadModel>>();
            builder.RegisterType<ReadModelFactory<AppointmentInsertReadModel>>().As<IReadModelFactory<AppointmentInsertReadModel>>();
            builder.RegisterType<AggregateReadStoreManager<AppointmentAggregate, AppointmentId, MongoDbReadModelStore<AppointmentReadModel>, AppointmentReadModel>>();
            ISnapshotStrategy newStrategy = SnapshotEveryFewVersionsStrategy.With(10);
            builder.RegisterInstance(newStrategy);
            // builder.RegisterType<EventFlowOptionsSnapshotExtensions>
            builder.Populate(new ServiceCollection());

            KafkaServicesRegistration.Register(builder);
        }
    }
}