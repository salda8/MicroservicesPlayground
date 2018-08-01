using EventFlow.ValueObjects;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace AppointmentApi.Models.AppointmentModel.ValueObjects
{
    public class Car : ValueObject
    {
        public string Type { get; set; }
        public long Kilometers { get; set; }
        public string RegisteredPlate { get; set; }
        public DateTime RegisteredDate { get; set; }
        public string Manufacturer { get; set; }

[BsonConstructor]
        public Car(string manufacturer, string type, long kilometers, string registeredPlate, DateTime registeredDate)
        {
            RegisteredDate = registeredDate;
            RegisteredPlate = registeredPlate;
            Kilometers = kilometers;
            Type = type;
            Manufacturer = manufacturer;
        }

        
    }
}