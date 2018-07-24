// using AppointmentApi.AppointmentModel.Commands;
// using AppointmentApi.AppointmentModel.Events;
// using AppointmentApi.AppointmentModel.ValueObjects;
// using AppointmentApi.Models.CapacityPlanningModel;
// using EventFlow.Aggregates;
// using EventFlow.Sagas;
// using EventFlow.Sagas.AggregateSagas;
// using SchedulingApi.Controllers;
// using System.Threading;
// using System.Threading.Tasks;

// namespace AppointmentApi.Sagas
// {
//     public class AppointmentSaga : AggregateSaga<AppointmentSaga, AppointmentSagaId, AppointmentSagaLocator>, ISagaIsStartedBy<AppointmentAggregate, AppointmentId, ProposedTimeScheduledEvent>//, ISagaHandles<CapacityPlanningAggregate, CapacityPlanningId, ScheduleConfirmedEvent>
//     {
//         private Schedule proposedTimeSchedule;
//         private bool scheduleConfirmed;

//         protected AppointmentSaga(AppointmentSagaId id) : base(id)
//         {
//         }

//         public Task HandleAsync(IDomainEvent<AppointmentAggregate, AppointmentId, ProposedTimeScheduledEvent> domainEvent, ISagaContext sagaContext, CancellationToken cancellationToken)
//         {

//             Apply(new ProposedTimeScheduledEvent(domainEvent.AggregateEvent.Schedule));
//             Publish(new MakeReservationCommand(CapacityPlanningId.New));
//             return Task.FromResult(0);
//         }

//         public Task HandleAsync(IDomainEvent<CapacityPlanningAggregate, CapacityPlanningId, ScheduleConfirmedEvent> domainEvent, ISagaContext sagaContext, CancellationToken cancellationToken)
//         {
//             Apply(domainEvent.AggregateEvent);
//             Publish(new AppointmentSetScheduleConfirmedCommand(proposedTimeSchedule, new AppointmentId(base.Id.Value)));
//             Complete();

//             return Task.FromResult(scheduleConfirmed);
//         }

//         public void Apply(ProposedTimeScheduledEvent e)
//         {
//             proposedTimeSchedule = e.Schedule;
//         }

//         public void Apply(ScheduleConfirmedEvent e)
//         {
//             scheduleConfirmed = e.Confirmed;
//         }
//     }
// }