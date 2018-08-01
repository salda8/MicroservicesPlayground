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
    [EventVersion("ProposedTimeSchedule", 1)]
    public class ProposedTimeScheduledEvent : AggregateEvent<AppointmentAggregate, AppointmentId>
    {
        public ProposedTimeScheduledEvent(Schedule schedule)
        {
            Schedule = schedule;
        }

        public Schedule Schedule { get; }
    }
}
