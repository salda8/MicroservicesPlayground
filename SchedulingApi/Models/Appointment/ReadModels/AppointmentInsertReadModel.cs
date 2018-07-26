using AppointmentApi.AppointmentModel.ValueObjects;
using EventFlow.Aggregates;
using EventFlow.MongoDB.ReadStores;
using EventFlow.MongoDB.ReadStores.Attributes;
using EventFlow.ReadStores;
using MongoDB.Bson.Serialization.Attributes;
using SchedulingApi.Controllers;

namespace AppointmentApi
{
    [MongoDbCollectionName("appointmentsInsert")]
    public class AppointmentInsertReadModel : IMongoDbInsertOnlyReadModel,
    IAmReadModelFor<AppointmentAggregate, AppointmentId, AppointmentBookedEvent>
    {
        public Location Location { get; private set; }
        public Schedule Schedule { get; private set; }
        public AppointmentId Id { get; private set; }
        public CarService CarService { get; private set; }

        [BsonId]
        object IMongoDbInsertOnlyReadModel._id { get; set; }

        public void Apply(IReadModelContext context, IDomainEvent<AppointmentAggregate, AppointmentId, AppointmentBookedEvent> domainEvent)
        {
            Id = domainEvent.AggregateIdentity;

            Location = domainEvent.AggregateEvent.Location;
            Schedule = domainEvent.AggregateEvent.Schedule;
            CarService = domainEvent.AggregateEvent.CarService;
        }
    }
}