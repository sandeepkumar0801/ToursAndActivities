using CacheManager.Constants;
using CacheManager.Contract;
using CacheManager.Helper;
using Isango.Entities;
using Isango.Entities.Wrapper;
using Util;

namespace CacheManager.FareHarborCacheManagers
{
    public class FareHarborCustomerTypesCacheManager : IFareHarborCustomerTypesCacheManager
    {
        #region Variable

        private readonly CollectionDataFactory<CacheKey<AgeGroup>> _collectionDataFactory;

        #endregion Variable

        #region Constructor

        public FareHarborCustomerTypesCacheManager(CollectionDataFactory<CacheKey<AgeGroup>> cosmosHelper)
        {
            _collectionDataFactory = cosmosHelper;
        }

        #endregion Constructor

        #region Implemented Methods

        public bool SetFareHarborAgeGroupsByActivityToCache(CacheKey<AgeGroup> cacheResult)
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

        #endregion Implemented Methods
    }
}