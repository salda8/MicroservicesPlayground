using AppointmentApi;
using EventFlow;
using EventFlow.Aggregates;
using EventFlow.Commands;
using EventFlow.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchedulingApi.Controllers
{

    public class AppointmentBookCommandHandler : CommandHandler<AppointmentAggregate, AppointmentId, AppointmentBookCommand>
    {
        public override Task ExecuteAsync(AppointmentAggregate aggregate, AppointmentBookCommand command, CancellationToken cancellationToken)
        {
            aggregate.OrderAppointment(command.Appointment);
            return Task.FromResult(0);
        }
    }
}