using AppointmentApi.AppointmentModel.Events;
using AppointmentApi.AppointmentModel.ValueObjects;
using EventFlow.Aggregates;

namespace SchedulingApi.Controllers
{
    public class AppointmentState : AggregateState<AppointmentAggregate, AppointmentId, AppointmentState>,
       
        IApply<LocationSetEvent>,
        //IApply<ProposedTimeScheduledEvent>,
        IApply<AppointmentCompletedEvent>,
        IApply<AppointmentOrderCreatedEvent>,
        IApply<CarServiceSetEvent>

    {
        public AppointmentOrder AppointmentOrder { get; private set; }
        public CarService CarService { get; set; }
        public Location Location { get; private set; }
        public Schedule Schedule { get;  set; }
        public AppointmentId Id { get; private set; }
        public bool Completed { get; private set; }

        public void Apply(AppointmentBookedEvent aggregateEvent)
        {
            AppointmentOrder = aggregateEvent.Order;
        }

        public void Apply(LocationSetEvent aggregateEvent)
        {
            Location = aggregateEvent.Location;
        }

        public void Apply(ProposedTimeScheduledEvent aggregateEvent)
        {
            Schedule = aggregateEvent.Schedule;
        }

        public void Apply(AppointmentOrderCreatedEvent aggregateEvent)
        {
        }

        public void Apply(CarServiceSetEvent aggregateEvent) {
            CarService = aggregateEvent.CarService;

        }

        public void Apply(AppointmentCompletedEvent aggregateEvent)
        {
            Completed = true;
        }
    }

    public class AppointmentCompletedEvent : IAggregateEvent<AppointmentAggregate, AppointmentId>
    {
    }
}