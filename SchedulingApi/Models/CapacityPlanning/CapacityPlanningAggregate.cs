using EventFlow.Aggregates;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;
using EventFlow.EventStores;
using EventFlow.MongoDB.EventStore;
using EventFlow.Snapshots;
using EventFlow.Snapshots.Stores;
using EventFlow.Snapshots.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AppointmentApi.Models.CapacityPlanningModel
{
    public class CapacityPlanningAggregate : SnapshotAggregateRootWithSequenceStore<CapacityPlanningAggregate, CapacityPlanningId, CapacityPlanningSnapshot>
    {
        private CapacityPlanningState state = new CapacityPlanningState();

        public CapacityPlanningAggregate(CapacityPlanningId id, ISnapshotPersistence snapshotPersistence, IMongoDbEventSequenceStore sequenceStore, ISnapshotStrategy snapshotStrategy, IEventStore eventStore, ISnapshotStore snapshotStore) : base(snapshotPersistence, sequenceStore, snapshotStrategy, eventStore, snapshotStore, id)
        {
            Register(state);
        }

        protected override Task<CapacityPlanningSnapshot> CreateSnapshotAsync(CancellationToken cancellationToken) {

            return Task.FromResult(new CapacityPlanningSnapshot());
        }

        internal Task<bool> MakeReservation(MakeReservationCommand command)
        {
            
            Emit(new ScheduleConfirmedEvent());
            return Task.FromResult(IsThisTimespotAvailable());
        }

        private bool IsThisTimespotAvailable()
        {
            return true;
        }
    }

    public class CapacityPlanningState : AggregateState<CapacityPlanningAggregate, CapacityPlanningId, CapacityPlanningState>, IApply<ScheduleConfirmedEvent>
    {
        private ScheduleConfirmedEvent scheduleConfirmedEvent;

        public void Apply(ScheduleConfirmedEvent aggregateEvent)
        {
            scheduleConfirmedEvent = aggregateEvent;
        }
    }

    public class MakeReservationCommand : Command<CapacityPlanningAggregate, CapacityPlanningId, IExecutionResult>
    {
        public MakeReservationCommand(CapacityPlanningId aggregateId) : base(aggregateId)
        {
        }


    }

    public class MakeReservationCommandHandler : CommandHandler<CapacityPlanningAggregate, CapacityPlanningId, MakeReservationCommand>
    {
        public override Task ExecuteAsync(CapacityPlanningAggregate aggregate, MakeReservationCommand command, CancellationToken cancellationToken)
        {
           return aggregate.MakeReservation(command);
           
        }
    }

[EventVersion("ScheduleConfirmedEvent", 1)]
    public class ScheduleConfirmedEvent : AggregateEvent<CapacityPlanningAggregate, CapacityPlanningId> {
        public bool Confirmed { get; set; }
    }
}
