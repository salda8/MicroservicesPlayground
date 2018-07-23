using AppointmentApi;
using AppointmentApi.AppointmentModel.ValueObjects;
using AppointmentApi.MongoDb;
using Autofac;
using Autofac.Extensions;
using Autofac.Extensions.DependencyInjection;
using EventFlow;
using EventFlow.Autofac.Extensions;
using EventFlow.Extensions;
using EventFlow.Kafka;
using EventFlow.Logs;
using EventFlow.MongoDB.Extensions;
using EventFlow.RabbitMQ;
using EventFlow.RabbitMQ.Extensions;
using EventFlow.Snapshots.Strategies;
using EventFlow.ValueObjects;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using SchedulingApi.Controllers;
using Swashbuckle.AspNetCore.Swagger;
using EventBus.Kafka;
using System;
using System.Reflection;
using EventBus;
using EventBus.Abstractions;
using AppointmentApi.Models.Appointment.Integration;
using EventFlow.Sagas;
using AppointmentApi.Sagas;
using EventFlow.Aggregates;

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
            builder.RegisterType<SagaUpdater<AppointmentAggregate, AppointmentId, AppointmentBookedEvent, AppointmentSaga>>().As<ISagaUpdater<AppointmentAggregate, AppointmentId, AppointmentBookedEvent, AppointmentSaga>>();
            
            ISnapshotStrategy newStrategy = SnapshotEveryFewVersionsStrategy.With(10);
            builder.RegisterInstance(newStrategy);
          // builder.RegisterType<EventFlowOptionsSnapshotExtensions>
            builder.Populate(new ServiceCollection());

            ServicesRegistration.Register(builder);
            
        }
    }

}