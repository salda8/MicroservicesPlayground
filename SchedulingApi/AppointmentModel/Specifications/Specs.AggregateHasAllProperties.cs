using EventFlow.Specifications;
using SchedulingApi.Controllers;
using System.Collections.Generic;

namespace AppointmentApi.AppointmentModel.Specifications
{
    public class AppointmentSpecification : ISpecification<AppointmentAggregate>
    {
        public bool IsSatisfiedBy(AppointmentAggregate obj)
        {
            return !(obj.Location is null && obj.Schedule is null);
        }

        public IEnumerable<string> WhyIsNotSatisfiedBy(AppointmentAggregate obj)
        {
            if (IsSatisfiedBy(obj))
            {
                yield return string.Empty;
            }

            yield return $"Aggregate '{obj.Name}' with ID '{obj.GetIdentity()}' is missing properties";
        }
    }
}
