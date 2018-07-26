using EventFlow.ValueObjects;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentApi.Models.AppointmentModel.ValueObjects
{
    public class AdditionalDetails : SingleValueObject<string>
    {
         [BsonConstructor]
        public AdditionalDetails(string value) : base(value)
        {

        }
    }
}
