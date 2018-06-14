using AppointmentApi.AppointmentModel.ValueObjects;
using EventFlow.Entities;

namespace SchedulingApi.Controllers
{
    public class Appointment : Entity<AppointmentId>, EventFlow.MongoDB.ReadStores.IMongoDbInsertOnlyReadModel
    {
        private AppointmentId id;
        private AppointmentOrder appointmentOrder;

        public Appointment(AppointmentId id, AppointmentOrder appointmentOrder, Location location, Schedule schedule) :base(id)
        {
            this.id = id;
            _id = Id;
            this.appointmentOrder = appointmentOrder;
            Location = location;
            Schedule = schedule;
        }

        
        public string Id { get; set; }
        public string Name { get; set; }
        public Location Location { get; }
        public Schedule Schedule { get; }
        public object _id { get; set; } 
    }
}