using AppointmentApi.AppointmentModel.ValueObjects;
using EventFlow.Aggregates;
using EventFlow.Logs;
using EventFlow.MongoDB.ReadStores;
using EventFlow.MongoDB.ReadStores.Attributes;
using EventFlow.ReadStores;
using SchedulingApi.Controllers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.EventStores;
using EventFlow.Configuration;
using System;
using EventFlow.MongoDB.EventStore;

namespace AppointmentApi
{
    [MongoDbCollectionName("appointments")]
    public class AppointmentInsertReadModel : IMongoDbInsertOnlyReadModel,
    IAmReadModelFor<AppointmentAggregate, AppointmentId, AppointmentBookedEvent>

    {
        public AppointmentOrder AppointmentOrder { get; private set; }
        public Location Location { get; private set; }
        public Schedule Schedule { get; private set; }
        public AppointmentId Id { get; private set; }
              
        object IMongoDbInsertOnlyReadModel._id { get; set; }

        private readonly IEventPersistence _eventPersistence;
        private readonly IEventJsonSerializer _serializer;
        private readonly IMongoDbEventSequenceStore _eventSequenceStore;

        public AppointmentInsertReadModel()
        {

        }

        public AppointmentInsertReadModel(IEventPersistence eventPersistence, IEventJsonSerializer serializer, IMongoDbEventSequenceStore eventSequenceStore)
        {
            _eventPersistence = eventPersistence;
            _serializer = serializer;
            _eventSequenceStore = eventSequenceStore;
        }

        public void Apply(IReadModelContext context, IDomainEvent<AppointmentAggregate, AppointmentId, AppointmentBookedEvent> domainEvent)
        {
            Id = domainEvent.AggregateIdentity;
            //_id = Id.Value;
            AppointmentOrder = domainEvent.AggregateEvent.Order;
            Location = domainEvent.AggregateEvent.Location;
            Schedule = domainEvent.AggregateEvent.Schedule;
            var seq = (int)_eventSequenceStore.GetNextSequence("appointments");
           
            _eventPersistence.CommitEventsAsync(Id, new List<SerializedEvent>() { _serializer.Serialize(domainEvent) }, new CancellationToken());
        }

        //public Appointment ToAppointment()
        //{
        //    return new Appointment(

        //        Id, AppointmentOrder, Location, Schedule
        //    );
        //}
    }

    [MongoDbCollectionName("appointments")]
    public class AppointmentReadModel : IMongoDbReadModel,
        IAmReadModelFor<AppointmentAggregate, AppointmentId, AppointmentBookedEvent>

    {
        public AppointmentOrder AppointmentOrder { get; private set; }
        public Location Location { get; private set; }
        public Schedule Schedule { get; private set; }
        public AppointmentId Id { get; private set; }
        public string _id { get; private set; }
        public long? _version { get; set; }
        

        private readonly IEventPersistence _eventPersistence;
        private readonly IEventJsonSerializer _serializer;
        private readonly IMongoDbEventSequenceStore _eventSequenceStore;

        public AppointmentReadModel()
        {

        }

        public AppointmentReadModel(IEventPersistence eventPersistence, IEventJsonSerializer serializer, IMongoDbEventSequenceStore eventSequenceStore)
        {
            _eventPersistence = eventPersistence;
            _serializer = serializer;
            _eventSequenceStore = eventSequenceStore;
        }

        public void Apply(IReadModelContext context, IDomainEvent<AppointmentAggregate, AppointmentId, AppointmentBookedEvent> domainEvent)
        {
            Id = domainEvent.AggregateIdentity;
           
            AppointmentOrder = domainEvent.AggregateEvent.Order;
            Location = domainEvent.AggregateEvent.Location;
            Schedule = domainEvent.AggregateEvent.Schedule;
            var seq = (int)_eventSequenceStore.GetNextSequence("appointments");
           
            _eventPersistence.CommitEventsAsync(Id, new List<SerializedEvent>() { _serializer.Serialize(domainEvent) }, new CancellationToken());
        }

        public Appointment ToAppointment()
        {
            return new Appointment(

                Id, AppointmentOrder, Location, Schedule
            );
        }
    }

    public class AppointmentReadStoreManager : ReadStoreManager<IMongoDbReadModelStore<AppointmentReadModel>, AppointmentReadModel>
    {
        protected AppointmentReadStoreManager(ILog log, IResolver resolver, IMongoDbReadModelStore<AppointmentReadModel> readModelStore, IReadModelDomainEventApplier readModelDomainEventApplier, IReadModelFactory<AppointmentReadModel> readModelFactory) : base(log, resolver, readModelStore, readModelDomainEventApplier, readModelFactory)
        {
        }

        protected override IReadOnlyCollection<ReadModelUpdate> BuildReadModelUpdates(IReadOnlyCollection<IDomainEvent> domainEvents)
        {
            throw new NotImplementedException();
        }

        protected override Task<ReadModelEnvelope<AppointmentReadModel>> UpdateAsync(IReadModelContext readModelContext, IReadOnlyCollection<IDomainEvent> domainEvents, ReadModelEnvelope<AppointmentReadModel> readModelEnvelope, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }


    
}