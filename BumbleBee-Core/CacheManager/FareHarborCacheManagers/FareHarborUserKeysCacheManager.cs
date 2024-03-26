using CacheManager.Constants;
using CacheManager.Contract;
using CacheManager.Helper;
using Isango.Entities;
using Isango.Entities.Wrapper;
using MongoDB.Driver;
using System.Collections.Generic;
using Util;

namespace CacheManager.FareHarborCacheManagers
{
    public class FareHarborUserKeysCacheManager : IFareHarborUserKeysCacheManager
    {
        #region Variable

        private readonly CollectionDataFactory<CacheKey<FareHarborUserKey>> _collectionDataFactory;

        #endregion Variable

        #region Constructor

        public FareHarborUserKeysCacheManager(CollectionDataFactory<CacheKey<FareHarborUserKey>> collectionDataFactory)
        {
            _collectionDataFactory = collectionDataFactory;
        }

        #endregion Constructor

        #region Implemented Methods

        public bool SetAllFareHarborUserKeysToCache(CacheKey<FareHarborUserKey> cacheResult)
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

        public List<FareHarborUserKey> GetFareHarborUserKeys()
        {
            //for mongoDB
            var filter = Builders<CacheKey<FareHarborUserKey>>.Filter.Eq("_id", Constant.FareHarborUserKey);
            //end mongo
            var cache = _collectionDataFactory.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), $"select * from c where c.id='{Constant.FareHarborUserKey}'", filter);
            var result = cache?.CacheValue;

            return result;
        }

        #endregion Implemented Methods
    }
}