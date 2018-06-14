using EventFlow.MongoDB.ReadStores;
using EventFlow.Queries;
using EventFlow.ReadStores.InMemory;
using MongoDB.Driver;
using SchedulingApi.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AppointmentApi
{
    public class GetAllAppointmentsQueryHandler : IQueryHandler<GetAllAppointmentsQuery, IReadOnlyCollection<Appointment>>
    {
        private readonly IMongoDbReadModelStore<AppointmentReadModel> readStore;

        public GetAllAppointmentsQueryHandler(IMongoDbReadModelStore<AppointmentReadModel> readStore)
        {
            this.readStore = readStore;
        }
        public async Task<IReadOnlyCollection<Appointment>> ExecuteQueryAsync(GetAllAppointmentsQuery query, CancellationToken cancellationToken) {

            IAsyncCursor<AppointmentReadModel> readModels = await readStore.FindAsync(appointmentReadModel => true).ConfigureAwait(false);
            return readModels.ToEnumerable().Select(appointmentReadModel => appointmentReadModel.ToAppointment()).ToList();
        }
    }
}
