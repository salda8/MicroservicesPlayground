using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MongoDbGenericRepository;
using MongoDbGenericRepository.Models;

namespace SettingsApi.Repository
{
    public interface IGenericMongoRepository : IBaseMongoRepository
    {
       
        Task<bool> ReplaceDocumentAsync<TDocument>(TDocument modifiedDocument) where TDocument : IDocument;
        Task<TDocument> ReplaceOneAndGetAsync<TDocument>(string id, TDocument modifiedDocument) where TDocument : IDocument;
        //Task<TDocument> UpdateFieldsAndGetAsync<TDocument>(string id, List<Tuple<FieldDefinition<TDocument, object>, object>> fieldDefinitions, string partioningKey = null) where TDocument : IDocument;
        Task<bool> UpdateFieldsAsync<TDocument>(string id, Dictionary<string, object> updates) where TDocument : IDocument;
        Task<TDocument> UpdateFields<TDocument>(string id, Dictionary<string, object> updates) where TDocument : IDocument;
    }
}