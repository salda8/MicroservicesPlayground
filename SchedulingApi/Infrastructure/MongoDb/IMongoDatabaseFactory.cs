using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentApi.MongoDb
{
    public interface IMongoDatabaseFactory
    {
        IMongoDatabase Connect(IOptions<MongoConfigurationOptions> optionsAccessor);
    }

    public class MongoDatabaseFactory : IMongoDatabaseFactory
    {
        public IMongoDatabase Connect(IOptions<MongoConfigurationOptions> optionsAccessor)
        {
            var configurationOptions = optionsAccessor.Value;
            return new MongoClient(configurationOptions.MongoConnection).GetDatabase(configurationOptions.MongoDatabaseName);
        }
    }
}
