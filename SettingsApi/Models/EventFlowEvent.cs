using MongoDbGenericRepository.Attributes;
using MongoDbGenericRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SettingsApi.Models
{
    [CollectionName("eventFlowEvents")]
    public class EventFlowEvent : Document,IDocument
    {
       
        public string AggregateName { get; set; }
        public int AggregateSequenceNumber { get; set; }
        public Guid AggregateId { get; set; }
        public DateTime? UtcTimestamp { get; set; }
        public Properties Properties { get; set; }

    }

    public class Properties
    {
        public string FullName { get; set; }
        public string SourceContext { get; set; }
        public string MachineName { get; set; }
        public long ThreadId { get; set; }
        public long ProcessId { get; set; }
        public string EnvironmentUserName { get; set; }
        public string Application { get; set; }
    }
}
