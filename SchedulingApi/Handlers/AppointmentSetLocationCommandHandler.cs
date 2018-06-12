using EventFlow.Commands;
using SchedulingApi.Controllers;
using System.Threading;
using System.Threading.Tasks;

namespace AppointmentApi.AppointmentModel.Commands
{
    public class AppointmentSetLocationCommandHandler : CommandHandler<AppointmentAggregate, AppointmentId, AppointmentSetLocationCommand>
    {
        public override Task ExecuteAsync(AppointmentAggregate aggregate, AppointmentSetLocationCommand command, CancellationToken cancellationToken)
        {
            aggregate.SetLocation(command.Location);
            return Task.FromResult(0);
        }
    }
}
