using System.Collections.Generic;
using System.Text;
using EventBus.Kafka;
using EventFlow.Kafka;

namespace SchedulingApi
{
    public class KafkaSettings 
    {
        public List<string> SubscribedTopics { get; set; }
        public string ClientId { get ; set ; }
        public string GroupId { get ; set ; }

        public List<string> BrokerAddresses {get;set;}

    }
}