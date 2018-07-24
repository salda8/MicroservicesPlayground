using AppointmentApi.AppointmentModel.ValueObjects;
using EventFlow.Commands;
using SchedulingApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppointmentApi.AppointmentModel.Commands
{
    public class AppointmentSetScheduleCommand : Command<AppointmentAggregate, AppointmentId>
    {

        public AppointmentSetScheduleCommand(Schedule schedule, AppointmentId id) : base(id)
        {
            Schedule = schedule;
        }

        public Schedule Schedule { get; }
    }

        public class AppointmentSetScheduleConfirmedCommand : Command<AppointmentAggregate, AppointmentId>
    {

        public AppointmentSetScheduleConfirmedCommand(Schedule schedule, AppointmentId id) : base(id)
        {
            Schedule = schedule;
        }

        public Schedule Schedule { get; }
    }
}
