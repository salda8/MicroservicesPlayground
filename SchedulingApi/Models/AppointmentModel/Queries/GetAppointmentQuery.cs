using EventFlow.Queries;
using SchedulingApi.Controllers;
using System.Collections.Generic;

namespace AppointmentApi
{
    public class GetAppointmentQuery : IQuery<IReadOnlyCollection<Appointment>>
    {
        public GetAppointmentQuery(params AppointmentId[] appointmentIds) : this((IEnumerable<AppointmentId>)appointmentIds)
        {

        }

        public GetAppointmentQuery(IEnumerable<AppointmentId> appointmentIds)
        {
            AppointmentIds = appointmentIds;
        }

        public IEnumerable<AppointmentId> AppointmentIds { get; }
    }
}
