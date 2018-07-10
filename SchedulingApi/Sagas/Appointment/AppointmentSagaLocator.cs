using AppointmentApi.AppointmentModel.Events;
using EventFlow.Aggregates;
using EventFlow.Sagas;
using EventFlow.Sagas.AggregateSagas;
using SchedulingApi.Controllers;
using System.Threading;
using System.Threading.Tasks;

namespace AppointmentApi.Sagas
{
    public class AppointmentSagaLocator : ISagaLocator
    {
        public Task<ISagaId> LocateSagaAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var appointmentId = domainEvent.Metadata["appointment-id"];
            var appointmentSagaId = new AppointmentSagaId($"appointmentSaga-{appointmentId}");

            return Task.FromResult<ISagaId>(appointmentSagaId);
        }
    }

    internal class AppointmentSagaId : ISagaId
    {
       public AppointmentSagaId(string id)
        {
            Value = id;
        }

        public string Value { get; }
    }

    internal class AppointmentSaga : AggregateSaga<AppointmentSaga, AppointmentSagaId, AppointmentSagaLocator>, ISagaIsStartedBy<AppointmentAggregate, AppointmentId, AppointmentBookedEvent>, ISagaHandles<AppointmentAggregate, AppointmentId, ScheduleSetEvent>
    {
        protected AppointmentSaga(AppointmentSagaId id) : base(id)
        {
        }

        public Task HandleAsync(IDomainEvent<AppointmentAggregate, AppointmentId, AppointmentBookedEvent> domainEvent, ISagaContext sagaContext, CancellationToken cancellationToken) {
            throw new System.NotImplementedException();
        }

        public Task HandleAsync(IDomainEvent<AppointmentAggregate, AppointmentId, ScheduleSetEvent> domainEvent, ISagaContext sagaContext, CancellationToken cancellationToken) => throw new System.NotImplementedException();
    }
}