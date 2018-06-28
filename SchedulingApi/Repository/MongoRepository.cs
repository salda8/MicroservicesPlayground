
using AppointmentApi;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdentityServer4.Quickstart.Repository
{
    /// <summary>
    /// Provides functionality  to persist "IdentityServer4.Models" into a given MongoDB
    /// </summary>
    public class MongoRepository : IRepository
    {
        protected static IMongoClient client;
        protected static IMongoDatabase database;

        /// <summary>
        /// This Contructor leverages  .NET Core built-in DI
        /// </summary>
        /// <param name="optionsAccessor">Injected by .NET Core built-in Depedency Injection</param>
        public MongoRepository(IOptions<MongoConfigurationOptions> optionsAccessor)
        {
            var configurationOptions = optionsAccessor.Value;

            client = new MongoClient(configurationOptions.MongoConnection);
            database = client.GetDatabase(configurationOptions.MongoDatabaseName);
            
        }

        

        public IQueryable<T> All<T>() where T : class, new()
        {
            return database.GetCollection<T>(typeof(T).Name).AsQueryable();
        }

        public IQueryable<T> Where<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : class, new()
        {
            return All<T>().Where(expression);
        }

        public void Delete<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var result = database.GetCollection<T>(typeof(T).Name).DeleteMany(predicate);

        }
        public T Single<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : class, new()
        {
            return All<T>().Where(expression).SingleOrDefault();
        }

        public bool CollectionExists<T>() where T : class, new()
        {
            var collection = database.GetCollection<T>(typeof(T).Name);
            var filter = new BsonDocument();
            var totalCount = collection.Count(filter);
            return (totalCount > 0);

        }

        public void Add<T>(T item) where T : class, new()
        {
            database.GetCollection<T>(typeof(T).Name).InsertOne(item);
        }

        public void Add<T>(IEnumerable<T> items) where T : class, new()
        {
            database.GetCollection<T>(typeof(T).Name).InsertMany(items);
        }


       
    }
}
