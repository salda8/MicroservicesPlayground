using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates;
using EventFlow.Core;
using EventFlow.EventStores;
using EventFlow.Extensions;
using EventFlow.MongoDB.EventStore;
using EventFlow.Snapshots;
using EventFlow.Snapshots.Stores;
using EventFlow.Snapshots.Strategies;

namespace AppointmentApi.Models
{
    public abstract class SnapshotAggregateRootWithSequenceStore<TAggregate, TIdentity, TSnapshot> : SnapshotAggregateRoot<TAggregate, TIdentity, TSnapshot>, IAggregateRoot
            where TAggregate : SnapshotAggregateRoot<TAggregate, TIdentity, TSnapshot>
            where TIdentity : IIdentity
            where TSnapshot : ISnapshot
    {
        private const string EventSequenceTableName = "eventflow.events";
        private readonly List<IUncommittedEvent> uncommittedEvents = new List<IUncommittedEvent>();
        private IMongoDbEventSequenceStore sequenceStore;
        private ISnapshotPersistence snapshotPersistence;
        private IEventStore eventStore;
        private ISnapshotStore snapshotStore;
        public new IEnumerable<IUncommittedEvent> UncommittedEvents => uncommittedEvents;


        public SnapshotAggregateRootWithSequenceStore(ISnapshotPersistence snapshotPersistence, IMongoDbEventSequenceStore sequenceStore, ISnapshotStrategy snapshotStrategy, IEventStore eventStore, ISnapshotStore snapshotStore, TIdentity id) : base(id, snapshotStrategy)
        {
            this.sequenceStore = sequenceStore;
            this.snapshotPersistence = snapshotPersistence;
            this.eventStore = eventStore;
            this.snapshotStore = snapshotStore;
            
        }

        /// <summary>
        /// Emits the specified aggregate event.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="aggregateEvent">The aggregate event.</param>
        /// <param name="metadata">The metadata.</param>
        /// <exception cref="ArgumentNullException">aggregateEvent</exception>
        protected override void Emit<TEvent>(TEvent aggregateEvent, IMetadata metadata = null)
        {
            if (EqualityComparer<TEvent>.Default.Equals(aggregateEvent, default(TEvent)))
            {
                throw new ArgumentNullException(nameof(aggregateEvent));
            }
            //Version = (int)sequenceStore.GetNextSequence(EventSequenceTableName);
            var aggregateSequenceNumber  = Version+1;

            var eventId = EventId.NewDeterministic(
                GuidFactories.Deterministic.Namespaces.Events,
                $"{Id.Value}-v{aggregateSequenceNumber}");
            var now = DateTimeOffset.Now;
            var eventMetadata = new Metadata
            {
                Timestamp = now,
                AggregateSequenceNumber = (int)aggregateSequenceNumber, 
                AggregateName = Name.Value,
                AggregateId = Id.Value,
                EventId = eventId
            };
            eventMetadata.Add(MetadataKeys.TimestampEpoch, now.ToUnixTime().ToString());
            if (metadata != null)
            {
                eventMetadata.AddRange(metadata);
            }

            var uncommittedEvent = new UncommittedEvent(aggregateEvent, eventMetadata);

            ApplyEvent(aggregateEvent);
            uncommittedEvents.Add(uncommittedEvent);
        }

        /// <summary>
        /// Commits the asynchronous.
        /// </summary>
        /// <param name="eventStore">The event store.</param>
        /// <param name="snapshotStore">The snapshot store.</param>
        /// <param name="sourceId">The source identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public override async Task<IReadOnlyCollection<IDomainEvent>> CommitAsync(IEventStore eventStore, ISnapshotStore snapshotStore, ISourceId sourceId, CancellationToken cancellationToken)
        {
            var domainEvents = await CommitDomainEventAsync(eventStore, sourceId, cancellationToken).ConfigureAwait(false);

            if (!await SnapshotStrategy.ShouldCreateSnapshotAsync(this, cancellationToken).ConfigureAwait(false))
            {
                return domainEvents;
            }


            var snapshotContainer = await CreateSnapshotContainerAsync(cancellationToken).ConfigureAwait(false);
            await snapshotStore.StoreSnapshotAsync<TAggregate, TIdentity, TSnapshot>(
                Id,
                snapshotContainer,
                cancellationToken)
                .ConfigureAwait(false);

            return domainEvents;
        }

        private Task<SnapshotContainer> CreateSnapshotContainerAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(new SnapshotContainer(CreateSnapshotAsync(cancellationToken).Result, CreateSnapshotMetadataAsync(cancellationToken).Result));
        }

        private async Task<IReadOnlyCollection<IDomainEvent>> CommitDomainEventAsync(IEventStore eventStore, ISourceId sourceId, CancellationToken cancellationToken)
        {
            if (eventStore == null) throw new ArgumentNullException(nameof(eventStore));

            var domainEvents = await eventStore.StoreAsync<TAggregate, TIdentity>(
                Id,
                uncommittedEvents,
                sourceId,
                cancellationToken)
                .ConfigureAwait(false);
            uncommittedEvents.Clear();
            return domainEvents;
        }

         protected override Task LoadSnapshotAsync(TSnapshot snapshot, ISnapshotMetadata metadata, CancellationToken cancellationToken)
        {
            return snapshotPersistence.GetSnapshotAsync(typeof(TAggregate), base.GetIdentity(), cancellationToken);
        }
        



    }
}