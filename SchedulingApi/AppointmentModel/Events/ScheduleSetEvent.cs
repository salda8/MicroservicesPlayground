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
    [EventVersion("ScheduleSet", 1)]
    public class ScheduleSetEvent : AggregateEvent<AppointmentAggregate, AppointmentId>
    {
        public ScheduleSetEvent(Schedule schedule)
        {
            Schedule = schedule;
        }

        public Schedule Schedule { get; }
    }
}
