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
    [EventVersion("LocationSet",1)]
    public class LocationSetEvent : AggregateEvent<AppointmentAggregate, AppointmentId>
    {
        public LocationSetEvent(Location location)
        {
            Location = location;
        }

        public Location Location { get; }
    }
}
