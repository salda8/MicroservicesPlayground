using EventFlow.ValueObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentApi.AppointmentModel.ValueObjects
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class CarService : SingleValueObject<string>
    {
        public CarService(string value) : base(value)
        {
            
        }
    }
}
