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
using AppointmentApi.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SchedulingApi.Controllers
{
    public class AppointmentAggregate : SnapshotAggregateRootWithSequenceStore<AppointmentAggregate, AppointmentId, AppointmentSnapshot>, ISnapshotAggregateRoot, IAggregateRoot
    {
        private AppointmentState state = new AppointmentState();
        public Schedule Schedule => state.Schedule;

        public void SetConfirmedSchedule(Schedule schedule)
        {
            state.Schedule = schedule;

        }

        public Location Location => state.Location;
        public AppointmentOrder AppointmentOrder => state.AppointmentOrder;
        public CarService CarService => state.CarService;
        public Task<IExecutionResult> BookAppointment(string value)
        {
            var spec = new AppointmentSpecification();
            //spec.ThrowDomainErrorIfNotSatisifiedBy(this);
            //await base.LoadAsync(eventStore, snapshotStore, CancellationToken.None);
            Emit(new AppointmentBookedEvent(AppointmentOrder, Location, Schedule, CarService));

            //this.eventFactory.Create(new AppointmentBookedEvent(AppointmentOrder, Location, Schedule),)

            return Task.FromResult(ExecutionResult.Success());
        }

        public AppointmentAggregate(AppointmentId id, ISnapshotPersistence snapshotPersistence, IMongoDbEventSequenceStore sequenceStore, ISnapshotStrategy snapshotStrategy, IEventStore eventStore, ISnapshotStore snapshotStore) : base(snapshotPersistence, sequenceStore, snapshotStrategy, eventStore, snapshotStore, id)
        {
            Register(state);
        }


        public void SetCarService(CarService carService)
        {
            Emit(new CarServiceSetEvent(carService));
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

        public void SetProposedSchedule(Schedule schedule)
        {
            Emit(new ProposedTimeScheduledEvent(schedule));
        }

        protected override Task<AppointmentSnapshot> CreateSnapshotAsync(CancellationToken cancellationToken)
        {
            //SnapshotContainer container = CreateSnapshotContainerAsync(cancellationToken).Result;
            //Task<SerializedSnapshot> serializedSnapshot = snapshotSerilizer.SerilizeAsync<AppointmentAggregate, AppointmentId, AppointmentSnapshot>(container, cancellationToken);
            //Task snapshot = snapshotPersistence.SetSnapshotAsync(typeof(AppointmentAggregate), base.GetIdentity(), serializedSnapshot.Result, cancellationToken);
            return Task.FromResult(new AppointmentSnapshot(Schedule, Location));
        }

        public void SetAppointmentAsCompleted()
        {
            Emit(new AppointmentCompletedEvent());
        }
    }
}