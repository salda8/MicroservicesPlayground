using EventFlow.Queries;
using EventFlow.ReadStores.InMemory;
using SchedulingApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AppointmentApi
{
    public class GetAppointmentQueryHandler : IQueryHandler<GetAppointmentQuery, IReadOnlyCollection<Appointment>>
    {
        private readonly IInMemoryReadStore<AppointmentReadModel> readStore;

        public GetAppointmentQueryHandler(IInMemoryReadStore<AppointmentReadModel> readStore)
        {
            this.readStore = readStore;
        }
        public async Task<IReadOnlyCollection<Appointment>> ExecuteQueryAsync(GetAppointmentQuery query, CancellationToken cancellationToken)
        {
            var appointmetnIds = new HashSet<AppointmentId>(query.AppointmentIds);
            var appointmentReadModels = await readStore.FindAsync(x => appointmetnIds.Contains(x.Id), cancellationToken);
            return appointmentReadModels.Select(x=>x.ToAppointment()).ToList();


        }
    }
}
