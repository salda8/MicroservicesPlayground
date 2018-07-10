using EventFlow.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentApi.Models.AppointmentModel.ValueObjects
{
    public class AdditionalDetails : SingleValueObject<string>
    {
        public AdditionalDetails(string value) : base(value)
        {

        }
    }
}
