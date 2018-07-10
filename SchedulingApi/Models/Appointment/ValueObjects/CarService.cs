using EventFlow.ValueObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentApi.AppointmentModel.ValueObjects
{
   
    public class CarService : ValueObject
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public CarService(string name, decimal price)
        {
            Price = price;
            Name = name;
        }
    }
}
