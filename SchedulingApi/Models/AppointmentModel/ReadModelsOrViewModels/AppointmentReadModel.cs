using AppointmentApi.AppointmentModel.ValueObjects;
using EventFlow.Aggregates;
using EventFlow.Configuration;
using EventFlow.Logs;
using EventFlow.MongoDB.ReadStores;
using EventFlow.MongoDB.ReadStores.Attributes;
using EventFlow.ReadStores;
using MongoDB.Bson.Serialization.Attributes;
using SchedulingApi.Controllers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppointmentApi
{
    [MongoDbCollectionName("appointmentsInsert")]
    public class AppointmentInsertReadModel : IMongoDbInsertOnlyReadModel,
    IAmReadModelFor<AppointmentAggregate, AppointmentId, AppointmentBookedEvent>

    {
        public AppointmentOrder AppointmentOrder { get; private set; }
        public Location Location { get; private set; }
        public Schedule Schedule { get; private set; }
        public AppointmentId Id { get; private set; }
        public CarService CarService { get; private set; }

        [BsonId]
        object IMongoDbInsertOnlyReadModel._id { get; set; }

        public void Apply(IReadModelContext context, IDomainEvent<AppointmentAggregate, AppointmentId, AppointmentBookedEvent> domainEvent)
        {
            Id = domainEvent.AggregateIdentity;
            //_id = Id.Value;
            AppointmentOrder = domainEvent.AggregateEvent.Order;
            Location = domainEvent.AggregateEvent.Location;
            Schedule = domainEvent.AggregateEvent.Schedule;
            CarService = domainEvent.AggregateEvent.CarService;
        }

        //public Appointment ToAppointment()
        //{
        //    return new Appointment(

        //        Id, AppointmentOrder, Location, Schedule
        //    );
        //}
    }

    [MongoDbCollectionName("appointmentsRead")]
    public class AppointmentReadModel : IMongoDbReadModel,
        IAmReadModelFor<AppointmentAggregate, AppointmentId, AppointmentBookedEvent>

    {
        public AppointmentOrder AppointmentOrder { get; private set; }
        public Location Location { get; private set; }
        public Schedule Schedule { get; private set; }
        public AppointmentId Id { get; private set; }
        public CarService CarService { get; private set; }
        [BsonIgnoreIfDefault]
        [BsonId]
        public string _id { get; private set; }
        public long? _version { get; set; }

        public void Apply(IReadModelContext context, IDomainEvent<AppointmentAggregate, AppointmentId, AppointmentBookedEvent> domainEvent)
        {
            Id = domainEvent.AggregateIdentity;

            AppointmentOrder = domainEvent.AggregateEvent.Order;
            Location = domainEvent.AggregateEvent.Location;
            Schedule = domainEvent.AggregateEvent.Schedule;
            CarService = domainEvent.AggregateEvent.CarService;
        }

        public Appointment ToAppointment()
        {
            return new Appointment(

                Id, AppointmentOrder, Location, Schedule, CarService
            );
        }
    }

    public class AppointmentReadStoreManager : ReadStoreManager<IMongoDbInsertOnlyReadModelStore<AppointmentInsertReadModel>, AppointmentInsertReadModel>
    {
        protected AppointmentReadStoreManager(ILog log, IResolver resolver, IMongoDbInsertOnlyReadModelStore<AppointmentInsertReadModel> readModelStore, IReadModelDomainEventApplier readModelDomainEventApplier, IReadModelFactory<AppointmentInsertReadModel> readModelFactory)
            : base(log, resolver, readModelStore, readModelDomainEventApplier, readModelFactory)
        {
        }

        protected override IReadOnlyCollection<ReadModelUpdate> BuildReadModelUpdates(IReadOnlyCollection<IDomainEvent> domainEvents)
        {
            // ReadModelFactory.CreateAsync("sdasd");
            throw new NotImplementedException();
        }

        protected override Task<ReadModelEnvelope<AppointmentInsertReadModel>> UpdateAsync(IReadModelContext readModelContext,
            IReadOnlyCollection<IDomainEvent> domainEvents, ReadModelEnvelope<AppointmentInsertReadModel> readModelEnvelope, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}