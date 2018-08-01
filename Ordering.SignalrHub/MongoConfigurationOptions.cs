using Ordering.SignalrHub;

namespace AppointmentApi
{
    
    public class MongoConfigurationOptions : IConnectionSetting
    {
        
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
    }
}
