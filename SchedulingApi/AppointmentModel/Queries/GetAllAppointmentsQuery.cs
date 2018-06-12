using EventFlow.Aggregates;
using EventFlow.Queries;
using EventFlow.ReadStores;
using SchedulingApi.Controllers;
using System.Collections.Generic;

namespace AppointmentApi
{
    public class GetAllAppointmentsQuery : IQuery<IReadOnlyCollection<Appointment>>
    {
        public GetAllAppointmentsQuery(params AppointmentId[] appointmentIds) : this((IEnumerable<AppointmentId>)appointmentIds)
        {

        }

        public GetAllAppointmentsQuery(IEnumerable<AppointmentId> appointmentIds)
        {
            AppointmentIds = appointmentIds;
        }

        public IEnumerable<AppointmentId> AppointmentIds { get; }
    }
}
