using AppointmentApi;
using AppointmentApi.AppointmentModel.Events;
using AppointmentApi.AppointmentModel.ValueObjects;
using EventFlow;
using EventFlow.Aggregates;
using EventFlow.Commands;
using EventFlow.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.MongoDB;
using EventFlow.EventStores;
using System.Collections.Generic;
using EventFlow.MongoDB.EventStore;

namespace SchedulingApi.Controllers
{
   
    public class AppointmentState : AggregateState<AppointmentAggregate, AppointmentId, AppointmentState>,
        IApply<AppointmentBookedEvent>,
        IApply<LocationSetEvent>,
        IApply<ScheduleSetEvent>,
        IApply<AppointmentOrderCreatedEvent>


    {
        private readonly IEventPersistence eventPersistence;
        private readonly IEventJsonSerializer serializer;
        private List<KeyValuePair<string, string>> metadat;
        private readonly IMongoDbEventSequenceStore eventSequenceStore;

        public AppointmentState(IEventPersistence eventPersistence, AppointmentId id, IEventJsonSerializer serializer, IMongoDbEventSequenceStore eventSequenceStore)
        {
            this.eventPersistence = eventPersistence;
            Id = id;
            this.serializer = serializer;
            this.eventSequenceStore = eventSequenceStore;
        }
        public AppointmentOrder AppointmentOrder { get; private set; }
        public Location Location { get; private set; }
        public Schedule Schedule { get; private set; }
        public AppointmentId Id { get; private set; }

        public void Apply(AppointmentBookedEvent aggregateEvent)
        {
            var seq = (int)eventSequenceStore.GetNextSequence("appointments");
            metadat = new List<KeyValuePair<string, string>>() {
               new KeyValuePair<string, string>(MetadataKeys.Timestamp, DateTime.Now.ToString()), new KeyValuePair<string, string>(MetadataKeys.AggregateSequenceNumber, seq.ToString())};
            var metadata = new Metadata(metadat);
           
           /// var domainEvent =  new List<SerializedEvent> { serializer.Serialize(new DomainEventFactory().Create(aggregateEvent,metadata , Id.Value, (int)eventSequenceStore.GetNextSequence("appointments"))) };
            AppointmentOrder = aggregateEvent.Order;

           // eventPersistence.CommitEventsAsync(Id, domainEvent, new CancellationToken() );
        }
        public void Apply(LocationSetEvent aggregateEvent) {
            Location = aggregateEvent.Location;
            
        }
        public void Apply(ScheduleSetEvent aggregateEvent) {
            Schedule = aggregateEvent.Schedule;
        }

        public void Apply(AppointmentOrderCreatedEvent aggregateEvent) {
            
            
        }
    }
}