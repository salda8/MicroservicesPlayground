namespace SchedulingApi
{
    internal class RabbitMqSettings : IConnectionSetting
    {
        public string ConnectionString { get; set; }
    }
}