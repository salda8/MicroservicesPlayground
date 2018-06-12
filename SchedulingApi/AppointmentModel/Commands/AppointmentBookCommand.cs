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

    public class AppointmentBookCommand : Command<AppointmentAggregate, AppointmentId>
    {

        public AppointmentBookCommand(AppointmentId appointmentId, AppointmentOrder appointment) : base(appointmentId)
        {
            this.Appointment = appointment;
        }

        public AppointmentOrder Appointment { get; set; }
    }
}