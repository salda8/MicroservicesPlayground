using System.Collections.Generic;
using System.Threading.Tasks;
using SchedulingApi.Controllers;

namespace AppointmentApi
{
    public interface IAppointmentService
    {
        void AddNewAppointment();
        Task<IReadOnlyCollection<Appointment>> GetAllAppointmentsQuery();
    }
}