using System.Threading.Tasks;

namespace MicroservicesPlayground.EventBus.Abstractions
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }
}
