﻿using AppointmentApi.AppointmentModel.ValueObjects;
using EventFlow.Commands;
using SchedulingApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppointmentApi.AppointmentModel.Commands
{
    public class AppointmentSetCarServiceCommand : Command<AppointmentAggregate, AppointmentId>
    {
        public AppointmentSetCarServiceCommand(AppointmentId id, CarService carSerivce) : base(id)

        {
            CarService = carSerivce;
        }

        public CarService CarService { get; }
    }

   
}
