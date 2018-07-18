using System;
using System.Threading.Tasks;
using EventBus.Abstractions;
using AppointmentApi.Models.Appointment;
using EventBus.Events;
using EventFlow;
using SchedulingApi.Controllers;
using System.Threading;

namespace AppointmentApi.Models.Appointment.Integration
{
    public class IntegrationEventTest : IntegrationEvent
    {
        
    }

    public class IntegrationTestEventHandler : IIntegrationEventHandler<IntegrationEventTest>
    {
        private readonly ICommandBus commandBus;

        public IntegrationTestEventHandler(ICommandBus commandBus)
        {
            this.commandBus = commandBus;
        }
        public async Task Handle(IntegrationEventTest @event)
        {
            var command =  new AppointmentBookCommand(AppointmentId.With(@event.Id));
            await commandBus.PublishAsync(command, cancellationToken: CancellationToken.None);
            
           
        }
        
    }
}
