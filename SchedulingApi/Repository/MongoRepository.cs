using AppointmentApi;
using AppointmentApi.MongoDb;
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
        protected static IMongoDatabase database;


        /// <summary>
        /// Initializes a new instance of the <see cref="MongoRepository"/> class.
        /// </summary>
        /// <param name="databaseFactory">The database factory.</param>
        /// <param name="optionsAccessor">The options accessor.</param>
        public MongoRepository(IMongoDatabaseFactory databaseFactory, IOptions<MongoConfigurationOptions> optionsAccessor)
        {
            database = databaseFactory.Connect(optionsAccessor);
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
            var totalCount = collection.CountDocuments(filter);
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