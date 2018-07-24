using EventFlow.ValueObjects;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace AppointmentApi.AppointmentModel.ValueObjects
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class Location : SingleValueObject<string>
    { [BsonConstructor]
        public Location(string value) : base(value)
        {
        }
    }
}