using AppointmentApi.AppointmentModel.ValueObjects;
using EventFlow.Aggregates;
using EventFlow.EventStores;
using SchedulingApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentApi.AppointmentModel.Events
{

    [EventVersion("CarService", 1)]
    public class CarServiceSetEvent : AggregateEvent<AppointmentAggregate, AppointmentId>
    {
        public CarServiceSetEvent(CarService carService)
        {
            CarService = carService;
        }

        public CarService CarService { get; }
    }
}
