using System.Threading.Tasks;

namespace EventBusKafka.Abstractions
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }
}