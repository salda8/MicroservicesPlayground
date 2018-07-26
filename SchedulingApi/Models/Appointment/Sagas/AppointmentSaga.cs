using AppointmentApi.AppointmentModel.Commands;
using AppointmentApi.AppointmentModel.Events;
using AppointmentApi.AppointmentModel.ValueObjects;
using AppointmentApi.Models.CapacityPlanningModel;
using EventFlow.Aggregates;
using EventFlow.Commands;
using EventFlow.Sagas;
using EventFlow.Sagas.AggregateSagas;
using Payments.Domain.Orders;
using Payments.Domain.Orders.Commands;
using Payments.Domain.Orders.Events;
using SchedulingApi.Controllers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppointmentApi.Sagas
{
    public class AppointmentSaga : AggregateSaga<AppointmentSaga, AppointmentSagaId, AppointmentSagaLocator>, ISagaHandles<OrderAggregate, OrderId, OrderPaymentCompleted>, ISagaHandles<AppointmentAggregate, AppointmentId, AppointmentBookedEvent> ,ISagaIsStartedBy<AppointmentAggregate, AppointmentId, ProposedTimeScheduledEvent>, ISagaHandles<CapacityPlanningAggregate, CapacityPlanningId, ScheduleConfirmedEvent>
    {
        private Schedule proposedTimeSchedule;
        private bool scheduleConfirmed;
        private AppointmentBookedEvent bookedAppointment;
        private OrderId orderID;
        private bool completed;

        public AppointmentSaga(AppointmentSagaId id) : base(id)
        {
        }

        public Task HandleAsync(IDomainEvent<AppointmentAggregate, AppointmentId, ProposedTimeScheduledEvent> domainEvent, ISagaContext sagaContext, CancellationToken cancellationToken)
        {
            Apply(new ProposedTimeScheduledEvent(domainEvent.AggregateEvent.Schedule));
            Publish(new MakeReservationCommand(CapacityPlanningId.New));
            return Task.FromResult(0);
        }

        public Task HandleAsync(IDomainEvent<CapacityPlanningAggregate, CapacityPlanningId, ScheduleConfirmedEvent> domainEvent, ISagaContext sagaContext, CancellationToken cancellationToken)
        {
            Apply(domainEvent.AggregateEvent);
            Publish(new AppointmentSetScheduleConfirmedCommand(proposedTimeSchedule, new AppointmentId(base.Id.Value)));


            return Task.FromResult(scheduleConfirmed);
        }
        public void Apply(ProposedTimeScheduledEvent e)
        {
            proposedTimeSchedule = e.Schedule;
        }

        public void Apply(ScheduleConfirmedEvent e)
        {
            scheduleConfirmed = e.Confirmed;
        }

        public Task HandleAsync(IDomainEvent<AppointmentAggregate, AppointmentId, AppointmentBookedEvent> domainEvent, ISagaContext sagaContext, CancellationToken cancellationToken)
        {
            Apply(domainEvent.AggregateEvent);
            orderID = OrderId.New;
            Publish(new CreateOrderCommand(orderID, base.Id.Value));
            Publish(new AddProductToOrderCommand(orderID, bookedAppointment.CarService.Name, 1, bookedAppointment.CarService.Price));
            Publish(new ProcessToPaymentCommand(orderID));
            return Task.CompletedTask;

        }


        private void Apply(AppointmentBookedEvent aggregateEvent)
        {
            bookedAppointment = aggregateEvent;
        }

        public Task HandleAsync(IDomainEvent<OrderAggregate, OrderId, OrderPaymentCompleted> domainEvent, ISagaContext sagaContext, CancellationToken cancellationToken)
        {

            completed = true;
            Publish(new AppointmentCompletedCommand(AppointmentId.With(base.Id.Value)));
            return Task.CompletedTask;
        }

    }

    public class AppointmentCompletedCommand : Command<AppointmentAggregate, AppointmentId>
    {
        public AppointmentCompletedCommand(AppointmentId aggregateId) : base(aggregateId)
        {
        }
    }

    public class AppointmentCompletedCommandHandler : CommandHandler<AppointmentAggregate, AppointmentId, AppointmentCompletedCommand>
    {
        public override Task ExecuteAsync(AppointmentAggregate aggregate, AppointmentCompletedCommand command, CancellationToken cancellationToken)
        {
            aggregate.SetAppointmentAsCompleted();
            return Task.CompletedTask;
        }
    }
}