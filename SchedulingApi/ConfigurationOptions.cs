using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentApi
{
    
    public class MongoConfigurationOptions
    {
        public string MongoConnection { get; set; }
        public string MongoDatabaseName { get; set; }
    }
}
