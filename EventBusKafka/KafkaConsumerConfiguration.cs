using System.Collections.Generic;
using EventFlow.Kafka;

namespace EventBus.Kafka {
    public class KafkaConsumerConfiguration : KafkaConfiguration, IKafkaConsumerConfiguration {

        public KafkaConsumerConfiguration(string brokerAddress, string groupId, string clientId, Dictionary<string, object> configuration = null, System.Text.Encoding encoding = null): base(brokerAddress, configuration, encoding) {
            base.AddToConfigOrThrowIfNull(groupId, KafkaGlobalConfigurationConstants.GroupId);
            GroupId = groupId;
            base.AddToConfigOrThrowIfNull(clientId, KafkaGlobalConfigurationConstants.ClientId);
            ClientId = clientId;

        }

        public string ClientId { get; set; }
        public string GroupId { get; set; } = "eventflow_consumer";
    }
}