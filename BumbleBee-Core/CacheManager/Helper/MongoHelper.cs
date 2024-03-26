using CacheManager.Contract;
using Isango.Entities;
using Logger.Contract;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CacheManager.Helper
{
    public class MongoHelper<T> : IMongoHelper<T>
    {
        private static ILogger _log;

        public MongoHelper(ILogger log)
        {
            _log = log;
        }

        public Task<bool> InsertDocument(string collectionName, T document)
        {
            var Collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);

            try
            {
                Type T = document.GetType();
                if (T.GetProperty("LocationId") != null) //pickuplocationcollection
                {
                    var PropLocationId = T.GetProperty("LocationId");
                    var GetCollectionLocationId = PropLocationId.GetValue(document);
                    var PropActivityId = T.GetProperty("ActivityId");
                    var GetCollectionActivityId = PropActivityId.GetValue(document);
                    var DeleteFilter = Builders<T>.Filter.Eq("ProductId", GetCollectionLocationId);
                    DeleteFilter &= Builders<T>.Filter.Eq("AffiliateId", GetCollectionActivityId);
                    Collection.DeleteOneAsync(DeleteFilter).GetAwaiter().GetResult();
                }
                else if (T.GetProperty("Id") != null)
                {
                    var Prop = T.GetProperty("Id");
                    var GetCollectionId = Prop.GetValue(document);
                    var DeleteFilter = Builders<T>.Filter.Eq("_id", GetCollectionId);
                    Collection.DeleteOneAsync(DeleteFilter).GetAwaiter().GetResult();
                }
                else if (T.GetProperty("ProductId") != null)
                {
                    var PropProductId = T.GetProperty("ProductId");
                    var GetCollectionProductId = PropProductId.GetValue(document);
                    var PropAffiliateId = T.GetProperty("AffiliateId");
                    var GetCollectionAffiliateId = PropAffiliateId.GetValue(document);
                    var DeleteFilter = Builders<T>.Filter.Eq("ProductId", GetCollectionProductId);
                    DeleteFilter &= Builders<T>.Filter.Eq("AffiliateId", GetCollectionAffiliateId);
                    Collection.DeleteOneAsync(DeleteFilter).GetAwaiter().GetResult();
                }
                Collection.InsertOneAsync(document).GetAwaiter().GetResult();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MongoHelper",
                    MethodName = "InsertDocument",
                };
                _log.Error(isangoErrorEntity, ex);
                return Task.FromResult(false);
            }
        }

        public Task<bool> InsertManyDocuments(string collectionName, List<T> documents)
        {
            var Collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
            try
            {
                Collection.InsertManyAsync(documents).GetAwaiter().GetResult();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MongoHelper",
                    MethodName = "InsertManyDocuments",
                };
                _log.Error(isangoErrorEntity, ex);
                return Task.FromResult(false);
            }
        }

        public List<T> GetResultList(string collectionName, string query, FilterDefinition<T> filter = null)
        {
            var result = default(List<T>);
            try
            {
                //SemaphoreSlim semaphore = new SemaphoreSlim(1);
                //var workerTasks = new Task[1];
                //for (int i = 0; i < workerTasks.Length; i++)
                //{
                //    workerTasks[i] = Task.Run(async delegate
                //    {
                //        if (semaphore != null)
                //        {
                //          ection?.Find(filter)?.ToList();

                //        semaphore?.Release();  await semaphore.WaitAsync();
                //        }

                //        var _collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
                //        result = _coll
                //    });
                //}
                //Task allFinished = Task.WhenAll(workerTasks);

                var _collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
                result = _collection?.Find(filter)?.ToList();
            }
            catch
            {
            }
            if (result == null)
            {
                Task.Run(() =>
                            _log.Error(new Isango.Entities.IsangoErrorEntity
                            {
                                ClassName = "MongoHelper",
                                MethodName = "GetResultList",
                                Params = $"collectionName : {collectionName},query {query}, filter : {filter?.ToString()}",
                            }, new Exception("No data found in collection"))
                    );
            }
            return result;
            //throw new NotImplementedException();
        }

        public T GetResult(string collectionName, string query, FilterDefinition<T> filter = null)
        {
            var result = default(T);
            try
            {
                //if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
                //{
                //    // register class map for T
                //    BsonClassMap.RegisterClassMap<T>(cm =>
                //    {
                //        cm.AutoMap();
                //        cm.SetIgnoreExtraElements(true);
                //        //cm.set
                //    });
                //}

                var resultList = GetResultList(collectionName, query, filter);
                if (resultList?.Any() == true)
                {
                    result = resultList.FirstOrDefault();
                }
            }
            catch (System.Exception ex)
            {
                Task.Run(() =>
                            _log.Error(new Isango.Entities.IsangoErrorEntity
                            {
                                ClassName = "MongoHelper",
                                MethodName = "GetResult",
                                Params = $"collectionName : {collectionName}, filter : {filter.ToString()}",
                            }, ex)
                    );
            }
            return result;
            //var _collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
            //var objectId = new ObjectId(id);
            //var filter = Builders<T>.Filter.Eq("_id", objectId);
            //return _collection.Find(filter).FirstOrDefault();
            //throw new NotImplementedException();
        }

        public Task<bool> CheckIfDocumentExist(string collectionName, string id)
        {
            try
            {
                var Collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
                var filter = Builders<T>.Filter.Eq("_id", id);
                var document = Collection.Find(filter)?.ToList();
                if (document != null && document.Count > 0)
                {
                    return Task.FromResult(true);
                }
                else
                {
                    return Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MongoHelper",
                    MethodName = "CheckIfDocumentExist",
                };
                _log.Error(isangoErrorEntity, ex);
                return Task.FromResult(false);
            }
        }

        public Task<bool> DeleteCollection(string collectionName)
        {
            try
            {
                var Collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
                Collection.DeleteManyAsync(Builders<T>.Filter.Empty).GetAwaiter().GetResult();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MongoHelper",
                    MethodName = "DeleteCollection",
                };
                _log.Error(isangoErrorEntity, ex);
                return Task.FromResult(false);
            }
        }

        public Task<bool> CreateCollection(string collectionName, string partitionKey)
        {
            return Task.FromResult(true);
        }

        public Task<bool> DeleteDocument(string collectionName, string id, string partitionKey)
        {
            try
            {
                var Collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
                if (!string.IsNullOrEmpty(id))
                {
                    var filter = Builders<T>.Filter.Eq("_id", id);
                    Collection.DeleteOneAsync(filter).GetAwaiter().GetResult();
                }
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MongoHelper",
                    MethodName = "DeleteDocument",
                };
                _log.Error(isangoErrorEntity, ex);
                return Task.FromResult(false);
            }
        }

        public Task<bool> DeleteDocument(string collectionName, string id, int partitionKey)
        {
            try
            {
                var Collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
                if (!string.IsNullOrEmpty(id))
                {
                    var filter = Builders<T>.Filter.Eq("_id", id);
                    Collection.DeleteOneAsync(filter).GetAwaiter().GetResult();
                }
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MongoHelper",
                    MethodName = "DeleteDocument",
                };
                _log.Error(isangoErrorEntity, ex);
                return Task.FromResult(false);
            }
        }

        public Task<bool> CheckIfCollectionExist(string collectionName)
        {
            // Didn't remove try-catch as one condition is handled in catch block
            try
            {
                var tables = IsangoDocumentClient.Instance.MongoDatabase.ListCollectionNames().ToList();
                if (tables.Any(x => x.ToLowerInvariant() == collectionName.ToLowerInvariant()))
                {
                    return Task.FromResult(true);
                }
                else
                {
                    return Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MongoHelper",
                    MethodName = "CheckIfCollectionExist",
                };
                _log.Error(isangoErrorEntity, ex);
                return Task.FromResult(false);
            }
        }

        public Task<bool> UpdateDocument(string collectionName, T document)
        {
            try
            {
                Type T = document.GetType();
                PropertyInfo Prop = T.GetProperty("Id");
                var GetCollectionId = Prop.GetValue(document);
                var Collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
                var filter = Builders<T>.Filter.Eq("_id", GetCollectionId);
                Collection.FindOneAndReplace(filter, document);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MongoHelper",
                    MethodName = "UpdateDocument",
                };
                _log.Error(isangoErrorEntity, ex);
                return Task.FromResult(false);
            }
        }

        public Task<bool> DeleteOlderDocuments(string collectionName, long timeStamp)
        {
            try
            {
                var Collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
                var filter = Builders<T>.Filter.Lt("TimeStamp", timeStamp);
                Collection.DeleteManyAsync(filter).GetAwaiter().GetResult();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                var isangoErrorEntity = new IsangoErrorEntity
                {
                    ClassName = "MongoHelper",
                    MethodName = "DeleteDocument",
                };
                _log.Error(isangoErrorEntity, ex);
                return Task.FromResult(false);
            }
        }

        //Below are Generic Mongo Methods for Reference
        //private protected string GetCollectionName(Type documentType)
        //{
        //    return ((BsonCollectionAttribute)documentType.GetCustomAttributes(
        //            typeof(BsonCollectionAttribute),
        //            true)
        //        .FirstOrDefault())?.CollectionName;
        //}

        //public virtual IQueryable<T> AsQueryable(string collectionName)
        //{
        //    var _collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
        //    return _collection.AsQueryable();
        //}

        //public virtual IEnumerable<T> FilterBy(
        //    Expression<Func<T, bool>> filterExpression, string collectionName)
        //{
        //    var _collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
        //    return _collection.Find(filterExpression).ToEnumerable();
        //}

        //public virtual IEnumerable<TProjected> FilterBy<TProjected>(
        //    Expression<Func<T, bool>> filterExpression,
        //    Expression<Func<T, TProjected>> projectionExpression, string collectionName)
        //{
        //    var _collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
        //    return _collection.Find(filterExpression).Project(projectionExpression).ToEnumerable();
        //}

        //public virtual T FindOne(Expression<Func<T, bool>> filterExpression, string collectionName)
        //{
        //    var _collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
        //    return _collection.Find(filterExpression).FirstOrDefault();
        //}

        //public virtual Task<T> FindOneAsync(Expression<Func<T, bool>> filterExpression, string collectionName)
        //{
        //    var _collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
        //    return Task.Run(() => _collection.Find(filterExpression).FirstOrDefaultAsync());
        //}

        //public virtual T FindById(string id, string collectionName)
        //{
        //    var _collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
        //    var objectId = new ObjectId(id);
        //    var filter = Builders<T>.Filter.Eq(doc => doc.Id, objectId);
        //    return _collection.Find(filter).SingleOrDefault();
        //}

        //public virtual Task<T> FindByIdAsync(string id, string collectionName)
        //{
        //    var _collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
        //    return Task.Run(() =>
        //    {
        //        var objectId = new ObjectId(id);
        //        var filter = Builders<T>.Filter.Eq(doc => doc.Id, objectId);
        //        return _collection.Find(filter).SingleOrDefaultAsync();
        //    });
        //}

        //public virtual void InsertOne(T document, string collectionName)
        //{
        //    var _collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
        //    _collection.InsertOne(document);
        //}

        //public virtual Task InsertOneAsync(T document, string collectionName)
        //{
        //    var _collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
        //    return Task.Run(() => _collection.InsertOneAsync(document));
        //}

        //public void InsertMany(ICollection<T> documents,string collectionName)
        //{
        //    var _collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
        //    _collection.InsertMany(documents);
        //}

        //public virtual async Task InsertManyAsync(ICollection<T> documents, string collectionName)
        //{
        //    var _collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
        //    await _collection.InsertManyAsync(documents);
        //}

        //public void ReplaceOne(T document, string collectionName)
        //{
        //    var _collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
        //    var filter = Builders<T>.Filter.Eq(doc => doc.Id, document.Id);
        //    _collection.FindOneAndReplace(filter, document);
        //}

        //public virtual async Task ReplaceOneAsync(T document, string collectionName)
        //{
        //    var _collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
        //    var filter = Builders<T>.Filter.Eq(doc => doc.Id, document.Id);
        //    await _collection.FindOneAndReplaceAsync(filter, document);
        //}

        //public void DeleteOne(Expression<Func<T, bool>> filterExpression, string collectionName)
        //{
        //    var _collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
        //    _collection.FindOneAndDelete(filterExpression);
        //}

        //public Task DeleteOneAsync(Expression<Func<T, bool>> filterExpression, string collectionName)
        //{
        //    var _collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
        //    return Task.Run(() => _collection.FindOneAndDeleteAsync(filterExpression));
        //}

        //public void DeleteById(string id, string collectionName)
        //{
        //    var _collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
        //    var objectId = new ObjectId(id);
        //    var filter = Builders<T>.Filter.Eq(doc => doc.Id, objectId);
        //    _collection.FindOneAndDelete(filter);
        //}

        //public Task DeleteByIdAsync(string id, string collectionName)
        //{
        //    var _collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
        //    return Task.Run(() =>
        //    {
        //        var objectId = new ObjectId(id);
        //        var filter = Builders<T>.Filter.Eq(doc => doc.Id, objectId);
        //        _collection.FindOneAndDeleteAsync(filter);
        //    });
        //}

        //public void DeleteMany(Expression<Func<T, bool>> filterExpression, string collectionName)
        //{
        //    var _collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
        //    _collection.DeleteMany(filterExpression);
        //}

        //public Task DeleteManyAsync(Expression<Func<T, bool>> filterExpression, string collectionName)
        //{
        //    var _collection = IsangoDocumentClient.Instance.MongoDatabase.GetCollection<T>(collectionName);
        //    return Task.Run(() => _collection.DeleteManyAsync(filterExpression));
        //}
    }
}