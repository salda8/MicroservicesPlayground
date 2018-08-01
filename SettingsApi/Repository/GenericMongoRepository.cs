using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDbGenericRepository;
using MongoDbGenericRepository.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SettingsApi.Repository
{
    /// <summary>
    /// Provides functionality to persist into a given MongoDB
    /// </summary>
    /// <seealso cref="MongoDbGenericRepository.BaseMongoRepository" />
    public class GenericMongoRepository : BaseMongoRepository, IGenericMongoRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericMongoRepository" /> class.
        /// </summary>
        /// <param name="optionsAccessor">The options accessor.</param>
        public GenericMongoRepository(IOptions<MongoConfigurationOptions> optionsAccessor) : base(optionsAccessor.Value.ConnectionString, optionsAccessor.Value.DatabaseName)
        {
        }
    }
}