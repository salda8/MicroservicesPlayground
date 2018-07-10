using EventFlow.Commands;

namespace SchedulingApi.Controllers
{
    public class AppointmentBookCommand : Command<AppointmentAggregate, AppointmentId>
    {
        public AppointmentBookCommand(AppointmentId appointmentId) : base(appointmentId)
        {
            //this.Appointment = appointment;
        }

        // public AppointmentOrder Appointment { get; set; }
    }
}