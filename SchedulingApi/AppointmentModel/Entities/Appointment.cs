using EventFlow.Entities;

namespace SchedulingApi.Controllers
{
    public class Appointment : Entity<AppointmentId>
    {
        private AppointmentId id;
        private AppointmentOrder appointmentOrder;

        public Appointment(AppointmentId id, AppointmentOrder appointmentOrder) :base(id)
        {
            this.id = id;
            this.appointmentOrder = appointmentOrder;
        }

        public string Id { get; set; }
        public string Name { get; set; }
    }
}