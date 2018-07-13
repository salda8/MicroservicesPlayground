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

        public ReturnDocument ReturnDocumentRule { get; set; } = ReturnDocument.After;

        public Task<TDocument> ReplaceOneAndGetAsync<TDocument>(string id, TDocument modifiedDocument)
            where TDocument : IDocument

        {
            var filter = Builders<TDocument>.Filter.Eq("Id", String.IsNullOrWhiteSpace(id) ? modifiedDocument.Id : throw new ArgumentNullException(nameof(id)));
            return HandlePartitioned(modifiedDocument).FindOneAndReplaceAsync(filter, modifiedDocument, new FindOneAndReplaceOptions<TDocument, TDocument> { ReturnDocument = ReturnDocumentRule });
        }

        public Task<TDocument> UpdateFields<TDocument>(string id, Dictionary<string, object> updates) where TDocument : IDocument
        {
            List<Tuple<FieldDefinition<TDocument, object>, object>> list = ConvertToFieldDefinitions<TDocument>(updates);

            return UpdateFieldsAndGetAsync(id, list);
        }

        private static List<Tuple<FieldDefinition<TDocument, object>, object>> ConvertToFieldDefinitions<TDocument>(Dictionary<string, object> updates) where TDocument : IDocument
        {
            var list = new List<Tuple<FieldDefinition<TDocument, object>, object>>();
            foreach (var item in updates.Keys)
            {
                FieldDefinition<TDocument, object> fieldDef = item;
                list.Add(Tuple.Create(fieldDef, updates[item]));
            }

            return list;
        }

        public async Task<bool> UpdateFieldsAsync<TDocument>(string id, Dictionary<string, object> updates) where TDocument : IDocument
        {
            UpdateDefinition<TDocument> updateDefinition = BuildUpdateDefinition(ConvertToFieldDefinitions<TDocument>(updates));
            var document = await GetByIdAsync<TDocument, string>(id).ConfigureAwait(false);
            return await base.UpdateOneAsync(document, updateDefinition).ConfigureAwait(false);
        }

        public Task<bool> ReplaceDocumentAsync<TDocument>(TDocument modifiedDocument) where TDocument : IDocument
        {
            return base.UpdateOneAsync(modifiedDocument);
        }

        private static UpdateDefinition<TDocument> BuildUpdateDefinition<TDocument>(List<Tuple<FieldDefinition<TDocument, object>, object>> fieldDefinitions) where TDocument : IDocument
        {
            int fieldDefCount = fieldDefinitions.Count;
            var updateDefinitions = new UpdateDefinition<TDocument>[fieldDefCount];
            var updateBuilder = Builders<TDocument>.Update;

            for (int i = 0; i < fieldDefCount; i++)
            {
                var fieldDef = fieldDefinitions[i];
                updateDefinitions[i] = updateBuilder.Set(fieldDef.Item1, fieldDef.Item2);
            }

            return updateBuilder.Combine(updateDefinitions);
        }

        internal Task<TDocument> UpdateFieldsAndGetAsync<TDocument>(string id, List<Tuple<FieldDefinition<TDocument, object>, object>> fieldDefinitions, string partioningKey = null) where TDocument : IDocument
        {
            UpdateDefinition<TDocument> updateDefinition = BuildUpdateDefinition(fieldDefinitions);
            var filter = Builders<TDocument>.Filter.Eq("Id", String.IsNullOrWhiteSpace(id) ? throw new ArgumentNullException(nameof(id)) : id);
            return base.HandlePartitioned<TDocument>(partioningKey).FindOneAndUpdateAsync<TDocument>(filter, updateDefinition, new FindOneAndUpdateOptions<TDocument, TDocument>() { ReturnDocument = ReturnDocumentRule });
        }
    }
}