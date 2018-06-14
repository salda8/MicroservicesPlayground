using EventFlow.Commands;
using SchedulingApi.Controllers;
using System.Threading;
using System.Threading.Tasks;

namespace AppointmentApi.AppointmentModel.Commands
{

    public class AppointmentOrderCreatedCommandHandler : CommandHandler<AppointmentAggregate, AppointmentId, AppointmentOrderCreateCommand>
    {
        public override Task ExecuteAsync(AppointmentAggregate aggregate, AppointmentOrderCreateCommand command, CancellationToken cancellationToken)
        {
            aggregate.CreateNewAppointmentOrder();
            return Task.FromResult(0);
        }
    }
}
