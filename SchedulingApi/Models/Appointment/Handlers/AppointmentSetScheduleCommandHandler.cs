using EventFlow.Commands;
using SchedulingApi.Controllers;
using System.Threading;
using System.Threading.Tasks;

namespace AppointmentApi.AppointmentModel.Commands
{
    public class AppointmentSetScheduleCommandHandler : CommandHandler<AppointmentAggregate, AppointmentId, AppointmentSetScheduleCommand>
    {
        public override Task ExecuteAsync(AppointmentAggregate aggregate, AppointmentSetScheduleCommand command, CancellationToken cancellationToken)
        {
            aggregate.SetSchedule(command.Schedule);
            return Task.FromResult(0);
        }
    }
}
