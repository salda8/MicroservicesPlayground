using Newtonsoft.Json;

namespace EventBus.Kafka {
    public class Location {
        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }
    }
}