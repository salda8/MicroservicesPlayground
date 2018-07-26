using Ordering.SignalrHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentApi
{
    
    public class MongoConfigurationOptions : IConnectionSetting
    {
        
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
    }
}
