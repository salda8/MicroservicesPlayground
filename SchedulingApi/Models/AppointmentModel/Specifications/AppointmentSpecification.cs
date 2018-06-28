using EventFlow.Specifications;
using SchedulingApi.Controllers;
using System.Collections.Generic;
using EventFlow.Extensions;
using EventFlow.Aggregates.ExecutionResults;
using System.Linq;

namespace AppointmentApi.AppointmentModel.Specifications
{
    public class AppointmentSpecification : ISpecification<AppointmentAggregate>
    {
        public bool IsSatisfiedBy(AppointmentAggregate obj)
        {
            return !(obj.Location is null || obj.Schedule is null);
        }

        public IEnumerable<string> WhyIsNotSatisfiedBy(AppointmentAggregate obj)
        {
            if (IsSatisfiedBy(obj))
            {
                yield break;
            }
            else {
                yield return $"Aggregate '{obj.Name}' with ID '{obj.GetIdentity()}' is missing properties";
            }

           
        }

        public IExecutionResult ReturnResultAsExecutionResult(AppointmentAggregate obj)
        {
            return this.IsNotSatisfiedByAsExecutionResult(obj);
        }

        public void ThrowDomainErrorIfNotSatisifiedBy(AppointmentAggregate obj)
        {
            this.ThrowDomainErrorIfNotSatisfied(obj);
        }
    }
}
