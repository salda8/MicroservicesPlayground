using EventFlow.Queries;
using SchedulingApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentApi
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IQueryProcessor queryProcessor;

        public AppointmentService(IQueryProcessor queryProcessor)
        {
            this.queryProcessor = queryProcessor;
        }

        public Task<IReadOnlyCollection<Appointment>> GetAllAppointmentsQuery()
        {
            throw new NotImplementedException();
        }

        public void AddNewAppointment()
        {

        }
    }
}
