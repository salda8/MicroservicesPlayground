using AppointmentApi.AppointmentModel.ValueObjects;
using EventFlow.Aggregates;
using EventFlow.EventStores;

namespace SchedulingApi.Controllers
{
    [EventVersion("AppointmentBooked", 1)]
    public class AppointmentBookedEvent : AggregateEvent<AppointmentAggregate, AppointmentId>
    {
        public AppointmentOrder Order { get; private set; }
        public Location Location { get; private set; }
        public Schedule Schedule { get; private set; }

        public AppointmentBookedEvent(AppointmentOrder order, Location location, Schedule schedule)
        {
            Order = order;
            Location = location;
            Schedule = schedule;
        }
    }
}