using EventFlow.Snapshots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AppointmentApi.Models.CapacityPlanningModel
{
    public class CapacityPlanningAggregate : SnapshotAggregateRoot<CapacityPlanningAggregate, CapacityPlanningId, CapacityPlanningSnapshot>
    {
        protected CapacityPlanningAggregate(CapacityPlanningId id, EventFlow.Snapshots.Strategies.ISnapshotStrategy snapshotStrategy) : base(id, snapshotStrategy)
        {
        }

        protected override Task<CapacityPlanningSnapshot> CreateSnapshotAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
        protected override Task LoadSnapshotAsync(CapacityPlanningSnapshot snapshot, ISnapshotMetadata metadata, CancellationToken cancellationToken) => throw new NotImplementedException();
    }
}
