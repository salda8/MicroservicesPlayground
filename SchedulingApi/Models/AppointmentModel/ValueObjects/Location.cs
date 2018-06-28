using EventFlow.ValueObjects;
using Newtonsoft.Json;

namespace AppointmentApi.AppointmentModel.ValueObjects
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class Location : SingleValueObject<string>
    {
        public Location(string value) : base(value)
        {
        }
    }
}