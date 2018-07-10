using AppointmentApi.AppointmentModel.ValueObjects;
using EventFlow.Snapshots;
using EventFlow.Snapshots.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AppointmentApi.AppointmentModel.Snapshots
{
    [SnapshotVersion("appointment", 1)]
    public class AppointmentSnapshot : ISnapshot
    {
        private Schedule schedule;

        public Schedule Schedule
        {
            get
            {
                return schedule;
            }
        }

        private Location location;

        public Location Location
        {
            get
            {
                return location;
            }
        }

        public AppointmentSnapshot(Schedule schedule, Location location)
        {
            this.schedule = schedule;
            this.location = location;
        }
    }

    


}
