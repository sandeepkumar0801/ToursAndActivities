using CacheManager.Constants;
using CacheManager.Contract;
using CacheManager.Helper;
using Isango.Entities;
using Isango.Entities.Wrapper;
using MongoDB.Driver;
using Util;

namespace CacheManager
{
    public class LandingCacheManager : ILandingCacheManager
    {
        #region Variables

        private readonly CollectionDataFactory<CacheKey<LocalizedMerchandising>> _collectionDataFactoryHelperLocalizedMerchandising;

        #endregion Variables

        #region Constructor

        public LandingCacheManager(CollectionDataFactory<CacheKey<LocalizedMerchandising>> collectionDataFactoryHelperLocalizedMerchandising)
        {
            _collectionDataFactoryHelperLocalizedMerchandising = collectionDataFactoryHelperLocalizedMerchandising;
        }

        #endregion Constructor

        public CacheKey<LocalizedMerchandising> GetLoadLocalizedMerchandising(string key)
        {
            var query = $"select VALUE M from {ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection")} M where M.id='{key}'";
            //for mongoDB
            var filter = Builders<CacheKey<LocalizedMerchandising>>.Filter.Eq("_id", key);
            //end mongo
            var result = _collectionDataFactoryHelperLocalizedMerchandising.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), query, filter);
            return result;
        }

        public bool SetLoadLocalizedMerchandising(CacheKey<LocalizedMerchandising> localizedMerchandising)
        {
            if (!_collectionDataFactoryHelperLocalizedMerchandising.GetCollectionDataHelper().CheckIfCollectionExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection")).Result)
            {
                _collectionDataFactoryHelperLocalizedMerchandising.GetCollectionDataHelper().CreateCollection(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), Constant.PartitionKeyMasterCollection);
            }

            var result = _collectionDataFactoryHelperLocalizedMerchandising.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), localizedMerchandising);
            result.Wait();
            return result.Result;
        }
    }
}