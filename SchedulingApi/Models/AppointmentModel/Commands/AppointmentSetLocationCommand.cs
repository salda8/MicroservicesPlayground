using AppointmentApi.AppointmentModel.ValueObjects;
using EventFlow.Commands;
using SchedulingApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppointmentApi.AppointmentModel.Commands
{
    public class AppointmentSetLocationCommand : Command<AppointmentAggregate, AppointmentId>
    {
        public AppointmentSetLocationCommand(AppointmentId id, Location location):base(id)
            
        {
            Location = location;
        }

        public Location Location { get; }
    }

   
}
