using EventFlow.Queries;
using EventFlow.ReadStores.InMemory;
using SchedulingApi.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AppointmentApi
{
    public class GetAllAppointmentsQueryHandler : IQueryHandler<GetAllAppointmentsQuery, IReadOnlyCollection<Appointment>>
    {
        private readonly IInMemoryReadStore<AppointmentReadModel> readStore;

        public GetAllAppointmentsQueryHandler(IInMemoryReadStore<AppointmentReadModel> readStore)
        {
            this.readStore = readStore;
        }
        public async Task<IReadOnlyCollection<Appointment>> ExecuteQueryAsync(GetAllAppointmentsQuery query, CancellationToken cancellationToken) {
            var readModels = await readStore.FindAsync(appointmentReadModel => true, cancellationToken).ConfigureAwait(false);
            return readModels.Select(appointmentReadModel => appointmentReadModel.ToAppointment()).ToList();
        }
    }
}
