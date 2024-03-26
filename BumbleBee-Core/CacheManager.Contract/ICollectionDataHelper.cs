using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace CacheManager.Contract
{
    public interface ICollectionDataHelper<T>
    {
        List<T> GetResultList(string collectionName, string query, FilterDefinition<T> filter = null);

        T GetResult(string collectionName, string query, FilterDefinition<T> filter=null);

        Task<bool> InsertDocument(string collectionName, T document);

        Task<bool> CheckIfDocumentExist(string collectionName, string id);

        Task<bool> DeleteCollection(string collectionName);

        Task<bool> CreateCollection(string collectionName, string partitionKey);

        Task<bool> DeleteDocument(string collectionName, string id, string partitionKey);

        Task<bool> DeleteDocument(string collectionName, string id, int partitionKey);

        Task<bool> CheckIfCollectionExist(string collectionName);

        Task<bool> UpdateDocument(string collectionName, T document);
        Task<bool> InsertManyDocuments(string collectionName, List<T> documents);

        Task<bool> DeleteOlderDocuments(string collectionName, long timeStamp);
    }
}