using EventFlow.Core;
using EventFlow.ValueObjects;
using Newtonsoft.Json;

namespace AppointmentApi.Models.CapacityPlanningModel
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class CapacityPlanningId : Identity<CapacityPlanningId>
    {
        protected CapacityPlanningId(string value) : base(value)
        {
        }
    }
}