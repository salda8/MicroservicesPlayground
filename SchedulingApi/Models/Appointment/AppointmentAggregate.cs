using AppointmentApi.AppointmentModel.Events;
using AppointmentApi.AppointmentModel.Snapshots;
using AppointmentApi.AppointmentModel.Specifications;
using AppointmentApi.AppointmentModel.ValueObjects;
using EventFlow.Aggregates;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Core;
using EventFlow.EventStores;
using EventFlow.Extensions;
using EventFlow.MongoDB.EventStore;
using EventFlow.Snapshots;
using EventFlow.Snapshots.Stores;
using EventFlow.Snapshots.Strategies;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SchedulingApi.Controllers
{
    public class AppointmentAggregate : SnapshotAggregateRoot<AppointmentAggregate, AppointmentId, AppointmentSnapshot>, IAggregateRoot<AppointmentId>
    {
        private readonly ISnapshotStore snapshotStore;
        private readonly IEventStore eventStore;
        private readonly ISnapshotPersistence snapshotPersistence;

        private AppointmentState state = new AppointmentState();
        public Schedule Schedule => state.Schedule;
        public Location Location => state.Location;
        public AppointmentOrder AppointmentOrder => state.AppointmentOrder;
        public CarService CarService => state.CarService;
        public List<IUncommittedEvent> NotCommittedEvents = new List<IUncommittedEvent>();

        public Task<IExecutionResult> BookAppointment(string value)
        {
            var spec = new AppointmentSpecification();
            //spec.ThrowDomainErrorIfNotSatisifiedBy(this);
            //await base.LoadAsync(eventStore, snapshotStore, CancellationToken.None).ConfigureAwait(false);
            Emit(new AppointmentBookedEvent(AppointmentOrder, Location, Schedule, CarService));

            //this.eventFactory.Create(new AppointmentBookedEvent(AppointmentOrder, Location, Schedule),)

            return Task.FromResult(ExecutionResult.Success());
        }

        private readonly IMongoDbEventSequenceStore sequenceStore;

        public AppointmentAggregate(AppointmentId id, ISnapshotPersistence snapshotPersistence, IMongoDbEventSequenceStore sequenceStore, ISnapshotStrategy snapshotStrategy, IEventStore eventStore, ISnapshotStore snapshotStore) : base(id, snapshotStrategy)
        {
            this.sequenceStore = sequenceStore;
            Register(state);

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

            var aggregateSequenceNumber = sequenceStore.GetNextSequence("appointments");

            var eventId = EventId.NewDeterministic(
                GuidFactories.Deterministic.Namespaces.Events,
                $"{Id.Value}-v{sequenceStore.GetNextSequence("eventflow.events")}");
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
            NotCommittedEvents.Add(uncommittedEvent);
        }

        public void SetCarService(CarService carService)
        {
            Emit(new CarServiceSetEvent(carService));
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
            await snapshotStore.StoreSnapshotAsync<AppointmentAggregate, AppointmentId, AppointmentSnapshot>(
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

            var domainEvents = await eventStore.StoreAsync<AppointmentAggregate, AppointmentId>(
                Id,
                NotCommittedEvents,
                sourceId,
                cancellationToken)
                .ConfigureAwait(false);
            NotCommittedEvents.Clear();
            return domainEvents;
        }

        public IExecutionResult CreateNewAppointmentOrder()
        {
            //Emit(new AppointmentOrderCreatedEvent());

            return ExecutionResult.Success();
        }

        public void SetLocation(Location location)
        {
            Emit(new LocationSetEvent(location));
        }

        public void SetSchedule(Schedule schedule)
        {
            Emit(new ScheduleSetEvent(schedule));
        }

        protected override Task<AppointmentSnapshot> CreateSnapshotAsync(CancellationToken cancellationToken)
        {
            //SnapshotContainer container = CreateSnapshotContainerAsync(cancellationToken).Result;
            //Task<SerializedSnapshot> serializedSnapshot = snapshotSerilizer.SerilizeAsync<AppointmentAggregate, AppointmentId, AppointmentSnapshot>(container, cancellationToken);
            //Task snapshot = snapshotPersistence.SetSnapshotAsync(typeof(AppointmentAggregate), base.GetIdentity(), serializedSnapshot.Result, cancellationToken);
            return Task.FromResult(new AppointmentSnapshot(Schedule, Location));
        }

        protected override Task LoadSnapshotAsync(AppointmentSnapshot snapshot, ISnapshotMetadata metadata, CancellationToken cancellationToken)
        {
            return snapshotPersistence.GetSnapshotAsync(typeof(AppointmentAggregate), base.GetIdentity(), cancellationToken);
        }
    }
}