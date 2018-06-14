using System;
using AppointmentApi.AppointmentModel.Events;
using AppointmentApi.AppointmentModel.Specifications;
using AppointmentApi.AppointmentModel.ValueObjects;
using EventFlow.Aggregates;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.EventStores;
using EventFlow.MongoDB.EventStore;

namespace SchedulingApi.Controllers
{
    public class AppointmentAggregate : AggregateRoot<AppointmentAggregate, AppointmentId>
    {
        private readonly IDomainEventFactory eventFactory;
        private AppointmentState state;
        public Schedule Schedule => state.Schedule;
        public Location Location => state.Location;
        public AppointmentOrder AppointmentOrder => state.AppointmentOrder;

        

        public IExecutionResult BookAppointment(string value) {
            var spec = new AppointmentSpecification();
            //spec.ThrowDomainErrorIfNotSatisifiedBy(this);
            Emit(new AppointmentBookedEvent(AppointmentOrder, Location, Schedule));
            //this.eventFactory.Create(new AppointmentBookedEvent(AppointmentOrder, Location, Schedule),)

            

            return ExecutionResult.Success();


        }

        public AppointmentAggregate(AppointmentId id, IDomainEventFactory eventFactory, IEventPersistence eventPersistence, IEventJsonSerializer jsonSerializer, IMongoDbEventSequenceStore sequenceStore) : base(id)
        {
            state = new AppointmentState(eventPersistence, id, jsonSerializer, sequenceStore);
            Register(state);
            this.eventFactory = eventFactory;
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
    }
}