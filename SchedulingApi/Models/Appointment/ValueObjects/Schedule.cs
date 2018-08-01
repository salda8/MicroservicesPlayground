using EventFlow.ValueObjects;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentApi.AppointmentModel.ValueObjects
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class Schedule : SingleValueObject<string>
    {
        [BsonConstructor]
        public Schedule(string value) : base(value)
        {
            
        }

        
    }
}
