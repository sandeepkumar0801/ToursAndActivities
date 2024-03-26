using CacheManager.Constants;
using CacheManager.Contract;
using CacheManager.Helper;
using Isango.Entities.Tiqets;
using Isango.Entities.Wrapper;
using MongoDB.Driver;
using System.Collections.Generic;
using Util;

namespace CacheManager.TiqetsCacheManager
{
    public class TiqetsPaxMappingCacheManager : ITiqetsPaxMappingCacheManager
    {
        #region Variable

        private readonly CollectionDataFactory<CacheKey<TiqetsPaxMapping>> _collectionDataFactory;

        #endregion Variable

        #region Constructor

        public TiqetsPaxMappingCacheManager(CollectionDataFactory<CacheKey<TiqetsPaxMapping>> cosmosHelper)
        {
            _collectionDataFactory = cosmosHelper;
        }

        #endregion Constructor

        #region Implemented Methods

        public bool SetPaxMappingToCache(CacheKey<TiqetsPaxMapping> cacheResult)
        {
            if (!_collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection")).Result)
            {
                _collectionDataFactory.GetCollectionDataHelper().CreateCollection(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), Constant.PartitionKeyMasterCollection);
            }

            if (_collectionDataFactory.GetCollectionDataHelper().CheckIfDocumentExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheResult.Id).Result)
            {
                return _collectionDataFactory.GetCollectionDataHelper().UpdateDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheResult).Result;
            }
            else
            {
                return _collectionDataFactory.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheResult).Result;
            }
        }

        public List<TiqetsPaxMapping> GetPaxMappings()
        {
            try
            {
                //for mongoDB
                var filter = Builders<CacheKey<TiqetsPaxMapping>>.Filter.Eq("_id", Constant.TiqetsPaxMapping);
                //end mongo
                var cache = _collectionDataFactory.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), $"select * from c where c.id='{Constant.TiqetsPaxMapping}'", filter);
                var result = cache?.CacheValue;

                return result;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        #endregion Implemented Methods
    }
}