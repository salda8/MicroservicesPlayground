using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SettingsApi
{
    
    public class MongoConfigurationOptions
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
    }
}
