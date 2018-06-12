using AppointmentApi.AppointmentModel.Events;
using AppointmentApi.AppointmentModel.Specifications;
using AppointmentApi.AppointmentModel.ValueObjects;
using EventFlow.Aggregates;

namespace SchedulingApi.Controllers
{
    public class AppointmentAggregate : AggregateRoot<AppointmentAggregate, AppointmentId>
    {
        private AppointmentState state = new AppointmentState();
        public Schedule Schedule => state.Schedule;
        public Location Location => state.Location;
        public AppointmentOrder AppointmentOrder => state.AppointmentOrder;

        public AppointmentAggregate(AppointmentId id) : base(id)
        {
            Register(state);
        }

        public void OrderAppointment(AppointmentOrder order)
        {
            if (new AppointmentSpecification().IsSatisfiedBy(this))
            {
                Emit(new AppointmentBookedEvent(order, Location, Schedule));
            }
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