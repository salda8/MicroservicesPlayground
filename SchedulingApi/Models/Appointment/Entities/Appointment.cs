using AppointmentApi.AppointmentModel.ValueObjects;
using EventFlow.Entities;

namespace SchedulingApi.Controllers
{
    public class Appointment : Entity<AppointmentId>
    {
        private AppointmentId id;
        private AppointmentOrder appointmentOrder;

        public CarService CarService { get; }

        public Appointment(AppointmentId id, AppointmentOrder appointmentOrder, Location location, Schedule schedule, CarService carService) : base(id)
        {
            CarService = carService;
            this.id = id;
            _id = Id;
            this.appointmentOrder = appointmentOrder;
            Location = location;
            Schedule = schedule;
        }


        public string Name { get; set; }
        public Location Location { get; }
        public Schedule Schedule { get; }
        public object _id { get; set; }
    }
}