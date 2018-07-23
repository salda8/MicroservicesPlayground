using AppointmentApi;
using EventFlow.Commands;
using EventFlow.EventStores;
using EventFlow.MongoDB.EventStore;
using EventFlow.MongoDB.ReadStores;
using EventFlow.Snapshots;
using EventFlow.Subscribers;
using System.Threading;
using System.Threading.Tasks;

namespace SchedulingApi.Controllers
{
    public class AppointmentBookCommandHandler : CommandHandler<AppointmentAggregate, AppointmentId, AppointmentBookCommand>
    {
        private readonly IMongoDbInsertOnlyReadModelStore<AppointmentInsertReadModel> mongoDbInsert;
        private readonly IDomainEventFactory eventFactory;
        private readonly ISnapshotStore snapshotStore;
        private readonly IEventStore eventStore;
        private readonly IEventJsonSerializer jsonSerializer;
        private readonly IMongoDbEventSequenceStore sequenceStore;
        private readonly IDomainEventPublisher domainEventPublisher;

        public AppointmentBookCommandHandler(IMongoDbInsertOnlyReadModelStore<AppointmentInsertReadModel> mongoDbInsert, IDomainEventPublisher domainEventPublisher, IDomainEventFactory eventFactory, ISnapshotStore snapshotStore, IEventStore eventStore, IEventJsonSerializer jsonSerializer, IMongoDbEventSequenceStore sequenceStore)
        {
            this.domainEventPublisher = domainEventPublisher;
            this.sequenceStore = sequenceStore;
            this.jsonSerializer = jsonSerializer;
            this.eventStore = eventStore;
            this.eventFactory = eventFactory;

            this.mongoDbInsert = mongoDbInsert;
            this.snapshotStore = snapshotStore;
        }

        public override Task ExecuteAsync(AppointmentAggregate aggregate, AppointmentBookCommand command, CancellationToken cancellationToken)
        {
            var result = aggregate.BookAppointment(command.AggregateId.Value);
            var committedEvents = aggregate.CommitAsync(eventStore: eventStore, snapshotStore: snapshotStore, sourceId: command.SourceId, cancellationToken: cancellationToken).Result;

            domainEventPublisher.PublishAsync(committedEvents, cancellationToken);
            // mongoDbInsert.UpdateAsync();
            return Task.FromResult(result);
        }
    }
}