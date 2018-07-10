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
}