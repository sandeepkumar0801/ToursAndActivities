using CacheManager.Constants;
using CacheManager.Contract;
using Isango.Entities;
using Logger.Contract;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Util;

namespace CacheManager.Helper
{
    public class CosmosHelper<T> : ICosmosHelper<T>
    {
        private readonly string cosmosDB = ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CosmosDb);
        private static ILogger _log;

        public CosmosHelper(ILogger log)
        {
            _log = log;
        }

        public List<T> GetResultList(string collectionName, string query, FilterDefinition<T> filter = null)
        {
            //var stopWatch = new Stopwatch();
            //stopWatch.Start();
            var queryOptions = new FeedOptions
            {
                MaxItemCount = -1,
                EnableCrossPartitionQuery = true,
                MaxDegreeOfParallelism = -1,
                MaxBufferedItemCount = -1,
                PopulateQueryMetrics = true
            };
            var result = IsangoDocumentClient.Instance.Client.CreateDocumentQuery<T>(
           //Need to do : Get DB from App Config
           UriFactory.CreateDocumentCollectionUri(cosmosDB, collectionName), query, queryOptions);
            var searchResult = result?.ToList();

            //stopWatch.Stop();
            //long duration = stopWatch.ElapsedMilliseconds;
            return searchResult;
        }

        /*public async Task<List<T>> GetResultListAsync(string collectionName, string query)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            FeedOptions opt = new FeedOptions
            {
                MaxItemCount = -1,
                EnableCrossPartitionQuery = true,
                MaxDegreeOfParallelism = 1024
            };
            IDocumentQuery<T> query1 = IsangoDocumentClient.Instance.Client.CreateDocumentQuery<T>(
                               UriFactory.CreateDocumentCollectionUri(cosmosDB, collectionName), query,
                               opt).AsDocumentQuery();

            List<T> results = new List<T>();
            while (query1.HasMoreResults)
            {
                results.AddRange(await query1.ExecuteNextAsync<T>());
            }
            var searchResult = results?.ToList();
            stopWatch.Stop();
            long duration = stopWatch.ElapsedMilliseconds;
            return searchResult;
        }*/

        public T GetResult(string collectionName, string query, FilterDefinition<T> filter=null)
        {
            var result = default(T);
            try
            {
                var searchResult = GetResultList(collectionName, query);
                if (searchResult != null)
                {
                    result = searchResult.FirstOrDefault();
                }
            }
            catch (System.Exception ex)
            {
                Task.Run(() =>
                            _log.Error(new Isango.Entities.IsangoErrorEntity
                            {
                                ClassName = "CosmosHelper",
                                MethodName = "GetResult",
                                Params = $"collectionName : {collectionName}, query : {query}",
                            }, ex)
                    );
            }
            return result;
        }

        public async Task<bool> InsertDocument(string collectionName, T document)
        {
            // Didn't remove try-catch as one condition is handled in catch block
            try
            {
                await IsangoDocumentClient.Instance.Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CosmosDb), collectionName), document);
                return true;
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.Conflict)
                {
                    await IsangoDocumentClient.Instance.Client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CosmosDb), collectionName), document);
                    return true;
                }

                return false;
            }
        }

        public async Task<bool> CheckIfDocumentExist(string collectionName, string id)
        {
            // Didn't remove try-catch as one condition is handled in catch block
            try
            {
                await IsangoDocumentClient.Instance.Client.ReadDocumentAsync(UriFactory.CreateDocumentUri(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CosmosDb), collectionName, id), new RequestOptions { PartitionKey = new PartitionKey(id) });
                return true;
            }
            catch (DocumentClientException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteCollection(string collectionName)
        {
            // Didn't remove try-catch as one condition is handled in catch block
            try
            {
                await IsangoDocumentClient.Instance.Client.DeleteDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CosmosDb), collectionName));
                return true;
            }
            catch (DocumentClientException)
            {
                return false;
            }
        }

        public async Task<bool> CreateCollection(string collectionName, string partitionKey)
        {
            // Didn't remove try-catch as one condition is handled in catch block
            try
            {
                var myCollection = new DocumentCollection() { Id = collectionName };
                myCollection.PartitionKey.Paths.Add(partitionKey);
                _log.Info($"CosmosHelper|CreateCollection|{IsangoDocumentClient.Instance.Client.ConnectionPolicy.EnableEndpointDiscovery}");
                await IsangoDocumentClient.Instance.Client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CosmosDb)), myCollection);
                return true;
            }
            catch (DocumentClientException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteDocument(string collectionName, string id, int partitionKey)
        {
            try
            {
                var url = UriFactory.CreateDocumentUri(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CosmosDb), collectionName, id);
                await IsangoDocumentClient.Instance.Client.DeleteDocumentAsync(url, new RequestOptions { PartitionKey = new PartitionKey(partitionKey) });
                return true;
            }
            catch (DocumentClientException)
            {
                return false;
            }
        }

        /// <summary>
        /// Delete Document from Cosmos Db when Id and PartitionKey are string
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteDocument(string collectionName, string id, string partitionKey)
        {
            try
            {
                var url = UriFactory.CreateDocumentUri(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CosmosDb), collectionName, id);
                await IsangoDocumentClient.Instance.Client.DeleteDocumentAsync(url, new RequestOptions { PartitionKey = new PartitionKey(partitionKey) });
                return true;
            }
            catch (DocumentClientException)
            {
                return false;
            }
        }

        public async Task<bool> CheckIfCollectionExist(string collectionName)
        {
            // Didn't remove try-catch as one condition is handled in catch block
            try
            {
                _log.Info($"CosmosHelper|CheckIfCollectionExist|{IsangoDocumentClient.Instance.Client.ConnectionPolicy.EnableEndpointDiscovery}");
                await IsangoDocumentClient.Instance.Client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CosmosDb), collectionName));
                return true;
            }
            catch (DocumentClientException)
            {
                return false;
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "CosmosHelper",
                    MethodName = "CheckIfCollectionExist",
                };
                _log.Error(isangoErrorEntity, ex);
                return false;
            }
        }

        public async Task<bool> UpdateDocument(string collectionName, T document)
        {
            try
            {
                var result = await IsangoDocumentClient.Instance.Client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CosmosDb), collectionName), document);
                return true;
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.Conflict)
                {
                    await IsangoDocumentClient.Instance.Client.ReplaceDocumentAsync(UriFactory.CreateDocumentCollectionUri(ConfigurationManagerHelper.GetValuefromAppSettings(Constant.CosmosDb), collectionName), document);
                    return true;
                }
            }

            return false;
        }

        public Task<bool> InsertManyDocuments(string collectionName, List<T> documents)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteOlderDocuments(string collectionName, long timeStamp)
        {
            throw new NotImplementedException();
        }
    }
}