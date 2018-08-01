using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EventBus.Kafka {
    public class KafkaEvent {
        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }


        public static KafkaEvent FromJson(string json)=> JsonConvert.DeserializeObject<KafkaEvent>(json, KafkaEvent.Settings);

        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
            new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    public static class Extensions {
        public static string ToJson(this KafkaEvent self)=> JsonConvert.SerializeObject(self, KafkaEvent.Settings);
    }
}