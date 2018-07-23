using System;

namespace EventBus.Events
{
    public class IntegrationEvent
    {
        public IntegrationEvent(string name)
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;

        }

        public Guid Id  { get; }
        public DateTime CreationDate { get; }

        public string Name { get; set; }

        
    }
}
