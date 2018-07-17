using Newtonsoft.Json;

namespace EventBus.Kafka {
    public partial class Payload {
        [JsonProperty("Location")]
        public Location Location { get; set; }
    }
}