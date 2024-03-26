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
    public class FareHarborCustomerPrototypesCacheManager : IFareHarborCustomerPrototypesCacheManager
    {
        #region Variable

        private readonly CollectionDataFactory<CacheKey<CustomerPrototype>> _collectionData;

        #endregion Variable

        #region Constructor

        public FareHarborCustomerPrototypesCacheManager(CollectionDataFactory<CacheKey<CustomerPrototype>> cosmosHelper)
        {
            _collectionData = cosmosHelper;
        }

        #endregion Constructor

        #region Implemented Methods

        public bool SetCustomerPrototypeByActivityToCache(CacheKey<CustomerPrototype> cacheResult)
        {
            if (!_collectionData.GetCollectionDataHelper().CheckIfCollectionExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection")).Result)
            {
                _collectionData.GetCollectionDataHelper().CreateCollection(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), Constant.PartitionKeyMasterCollection);
            }

            if (_collectionData.GetCollectionDataHelper().CheckIfDocumentExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheResult.Id).Result)
            {
                return _collectionData.GetCollectionDataHelper().UpdateDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheResult).Result;
            }
            else
            {
                return _collectionData.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheResult).Result;
            }
        }

        public List<CustomerPrototype> GetCustomerPrototypeList()
        {
            //for mongoDB
            var filter = Builders<CacheKey<CustomerPrototype>>.Filter.Eq("_id", Constant.FareHarborCustomerPrototype);
            //end mongo
            var cache = _collectionData.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), $"select * from c where c.id='{Constant.FareHarborCustomerPrototype}'", filter);
            var result = cache.CacheValue;

            return result;
        }

        #endregion Implemented Methods
    }
}