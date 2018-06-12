using AppointmentApi;
using AppointmentApi.AppointmentModel.Events;
using AppointmentApi.AppointmentModel.ValueObjects;
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

    public class AppointmentState : AggregateState<AppointmentAggregate, AppointmentId, AppointmentState>,
        IApply<AppointmentBookedEvent>,
        IApply<LocationSetEvent>,
        IApply<ScheduleSetEvent>


    {
        public AppointmentOrder AppointmentOrder { get; private set; }
        public Location Location { get; private set; }
        public Schedule Schedule { get; private set; }

        public void Apply(AppointmentBookedEvent aggregateEvent)
        {
            AppointmentOrder = aggregateEvent.Order;
        }
        public void Apply(LocationSetEvent aggregateEvent) {
            Location = aggregateEvent.Location;
        }
        public void Apply(ScheduleSetEvent aggregateEvent) {
            Schedule = aggregateEvent.Schedule;
        }
    }
}