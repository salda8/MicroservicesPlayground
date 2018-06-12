using AppointmentApi;
using EventFlow;
using EventFlow.Aggregates;
using EventFlow.Commands;
using EventFlow.Core;
using EventFlow.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchedulingApi.Controllers
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class AppointmentId : Identity<AppointmentId>
    {
        public AppointmentId(string value) : base(value)
        {
        }
    }
}