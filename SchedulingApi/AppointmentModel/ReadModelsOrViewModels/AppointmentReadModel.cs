using AppointmentApi.AppointmentModel.ValueObjects;
using EventFlow.Aggregates;
using EventFlow.ReadStores;
using SchedulingApi.Controllers;

namespace AppointmentApi
{
    public class AppointmentReadModel : IReadModel,
        IAmReadModelFor<AppointmentAggregate, AppointmentId, AppointmentBookedEvent>

    {
        public AppointmentOrder AppointmentOrder { get; private set; }
        public Location Location { get; private set; }
        public Schedule Schedule { get; private set; }
        public AppointmentId Id { get; private set; }

        public void Apply(IReadModelContext context, IDomainEvent<AppointmentAggregate, AppointmentId, AppointmentBookedEvent> domainEvent)
        {
            Id = domainEvent.AggregateIdentity;
            AppointmentOrder = domainEvent.AggregateEvent.Order;
            Location = domainEvent.AggregateEvent.Location;
            Schedule = domainEvent.AggregateEvent.Schedule;
        }

        public Appointment ToAppointment()
        {
            return new Appointment(

                Id, AppointmentOrder
            );
        }
    }
}