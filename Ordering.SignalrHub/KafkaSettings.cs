using System.Collections.Generic;

namespace Ordering.SignalrHub
{
    public class KafkaSettings 
    {
        public List<string> SubscribedTopics { get; set; }
        public string ClientId { get ; set ; }
        public string GroupId { get ; set ; }

        public List<string> BrokerAddresses {get;set;}

    }
}