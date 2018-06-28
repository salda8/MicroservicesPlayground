using EventFlow.Aggregates;
using EventFlow.EventStores;

namespace SchedulingApi.Controllers
{
    [EventVersion("AppointmentOrderCreated", 1)]
    public class AppointmentOrderCreatedEvent : AggregateEvent<AppointmentAggregate, AppointmentId>
    {
        public AppointmentOrderCreatedEvent(AppointmentId appointmentId) 
        {
        }
    }
}