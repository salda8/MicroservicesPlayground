using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace SchedulingApi.Controllers
{
    public class AppointmentOrderCreateCommand : Command<AppointmentAggregate, AppointmentId, IExecutionResult>
    {
        private AppointmentId appointmentId;

        public AppointmentOrderCreateCommand(AppointmentId appointmentId) : base(appointmentId)
        {
            this.appointmentId = appointmentId;
        }
    }
}