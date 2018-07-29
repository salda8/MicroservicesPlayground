using System;
using System.Threading.Tasks;
using MicroservicesPlayground.EventBus.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Ordering.SignalrHub.IntegrationEvents.Events;

namespace Ordering.SignalrHub.IntegrationEvents.EventHandling
{
    public class ScheduleConfirmedEventHandler : IIntegrationEventHandler<ScheduleConfirmedEvent>
    {
        private IHubContext<NotificationsHub> hubContext;

        public ScheduleConfirmedEventHandler(IHubContext<NotificationsHub> hubContext)
        {
            this.hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        public async Task Handle(ScheduleConfirmedEvent @event)
        {
           await hubContext.Clients
                .Group(@event.Id.ToString())
                .SendAsync("ScheduleConfirmation");
        }
    }
}