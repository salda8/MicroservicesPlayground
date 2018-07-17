using System;
using Newtonsoft.Json;

namespace EventBus.Kafka {
    public class Metadata {
        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [JsonProperty("aggregate_sequence_number")]
        public string AggregateSequenceNumber { get; set; }

        [JsonProperty("aggregate_name")]
        public string AggregateName { get; set; }

        [JsonProperty("aggregate_id")]
        public string AggregateId { get; set; }

        [JsonProperty("event_id")]
        public string EventId { get; set; }

        [JsonProperty("timestamp_epoch")]
        public string TimestampEpoch { get; set; }

        [JsonProperty("batch_id")]
        public string BatchId { get; set; }

        [JsonProperty("source_id")]
        public string SourceId { get; set; }

        [JsonProperty("event_name")]
        public string EventName { get; set; }

        [JsonProperty("event_version")]
        public string EventVersion { get; set; }
    }
}