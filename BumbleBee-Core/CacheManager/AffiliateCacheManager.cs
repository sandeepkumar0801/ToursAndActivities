using CacheManager.Constants;
using CacheManager.Contract;
using CacheManager.Helper;
using Isango.Entities.Affiliate;
using Isango.Entities.Wrapper;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using Util;

namespace CacheManager
{
    public class AffiliateCacheManager : IAffiliateCacheManager
    {
        #region Variable
        private readonly CollectionDataFactory<CacheKey<AffiliateFilter>> _collectionDataFactoryFilter;
        private readonly CollectionDataFactory<AffiliateFilter> _collectionDataFactoryAffiliateFilter;
        private readonly CollectionDataFactory<Affiliate> _collectionDataFactory;
        #endregion Variable

        #region Constructor

        public AffiliateCacheManager(
            CollectionDataFactory<CacheKey<AffiliateFilter>> collectionDataFactoryFilter,
            CollectionDataFactory<AffiliateFilter> collectionDataFactoryAffiliateFilter,
            CollectionDataFactory<Affiliate> collectionDataFactory)
        {
            _collectionDataFactoryFilter = collectionDataFactoryFilter;
            _collectionDataFactoryAffiliateFilter = collectionDataFactoryAffiliateFilter;
            _collectionDataFactory = collectionDataFactory;
        }

        #endregion Constructor

        #region Implemented Methods

        public Affiliate GetAffiliateInfo(string affiliateCacheKey)
        {
            var query = $"select VALUE M from {ConfigurationManagerHelper.GetValuefromAppSettings("AffiliateCollection")} M where M.id='{affiliateCacheKey}'";
            //for mongoDB
            var filter = Builders<Affiliate>.Filter.Eq("_id", affiliateCacheKey);
            //end mongo
            return _collectionDataFactory.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("AffiliateCollection"), query, filter);
        }

        public Affiliate GetAffiliateInfoV2(string affiliateCacheKey)
        {
            var collection = ConfigurationManagerHelper.GetValuefromAppSettings("AffiliateCollection");
            var query = $"select VALUE M from {collection} M where M.B2BAffiliateId='{affiliateCacheKey}'";
            //for mongoDB
            var filter = Builders<Affiliate>.Filter.Eq("_id", affiliateCacheKey);
            //end mongo
            return _collectionDataFactory.GetCollectionDataHelper().GetResult(collection, query, filter);
        }

        public bool SetAffiliateInfoToCache(Affiliate affiliateCacheResult)
        {
            var result = _collectionDataFactory.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("AffiliateCollection"), affiliateCacheResult);
            result.Wait();
            return result.Result;
        }

        public CacheKey<AffiliateFilter> GetAffiliateFilter(string affiliateCacheKey)
        {
            var query = $"select VALUE M from {ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection")} M where M.id='{affiliateCacheKey}'";
            //for mongoDB
            var filter = Builders<CacheKey<AffiliateFilter>>.Filter.Eq("_id", affiliateCacheKey);
            //end mongo
            return _collectionDataFactoryFilter.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), query, filter);
        }

        public bool SetAffiliateFilterToCache(CacheKey<AffiliateFilter> affiliateCacheResult)
        {
            if (!_collectionDataFactoryFilter.GetCollectionDataHelper().CheckIfCollectionExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection")).GetAwaiter().GetResult())
            {
                _collectionDataFactoryFilter.GetCollectionDataHelper().CreateCollection(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), Constant.PartitionKeyMasterCollection);
            }

            if (_collectionDataFactoryFilter.GetCollectionDataHelper().CheckIfDocumentExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), affiliateCacheResult.Id).GetAwaiter().GetResult())
            {
                return _collectionDataFactoryFilter.GetCollectionDataHelper().UpdateDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), affiliateCacheResult).GetAwaiter().GetResult();
            }

            return _collectionDataFactoryFilter.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), affiliateCacheResult).GetAwaiter().GetResult();
        }

        public bool SetAffiliateToCache(List<AffiliateFilter> affiliateFilters)
        {
            if (!_collectionDataFactoryFilter.GetCollectionDataHelper().CheckIfCollectionExist(ConfigurationManagerHelper.GetValuefromAppSettings("AffiliateFilterCollection")).GetAwaiter().GetResult())
            {
                _collectionDataFactoryAffiliateFilter.GetCollectionDataHelper().CreateCollection(ConfigurationManagerHelper.GetValuefromAppSettings("AffiliateFilterCollection"), Constant.AffiliatePartitionKey);
            }
            var count = 0;
            var result = false;
            foreach (var affilateFilter in affiliateFilters)
            {
                try
                {
                    affilateFilter.Id = affilateFilter.AffiliateId;
                    if (_collectionDataFactoryFilter.GetCollectionDataHelper().CheckIfDocumentExist(ConfigurationManagerHelper.GetValuefromAppSettings("AffiliateFilterCollection"), affilateFilter.Id).GetAwaiter().GetResult())
                    {
                        result = _collectionDataFactoryAffiliateFilter.GetCollectionDataHelper().UpdateDocument(ConfigurationManagerHelper.GetValuefromAppSettings("AffiliateFilterCollection"), affilateFilter).GetAwaiter().GetResult();
                    }
                    result = _collectionDataFactoryAffiliateFilter.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("AffiliateFilterCollection"), affilateFilter).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    count++;
                }
            }

            return count >= 0 && result;
        }

        public AffiliateFilter GetAffiliateFilterCache(string affiliateId)
        {
            try
            {
                var query = $"select * from M where M.affiliateId='{affiliateId}'";
                //for mongoDB
                var filter = Builders<AffiliateFilter>.Filter.Eq("_id", affiliateId);
                //end mongo
                return _collectionDataFactoryAffiliateFilter.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("AffiliateFilterCollection"), query, filter);
            }
            catch (Exception)
            {
                //ignored to run fall-back from db
            }
            return null;
        }

        public bool DeleteAndCreateCollection()
        {
            if (_collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist(ConfigurationManagerHelper.GetValuefromAppSettings("AffiliateCollection")).GetAwaiter().GetResult())
            {
                _collectionDataFactory.GetCollectionDataHelper().DeleteCollection(ConfigurationManagerHelper.GetValuefromAppSettings("AffiliateCollection")).Wait();
            }

            return _collectionDataFactory.GetCollectionDataHelper().CreateCollection(ConfigurationManagerHelper.GetValuefromAppSettings("AffiliateCollection"), Constant.PartitionKeyAffiliateCollection).GetAwaiter().GetResult();
        }

        public bool DeleteAndCreateAffiliateFilterCollection()
        {
            if (_collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist(ConfigurationManagerHelper.GetValuefromAppSettings("AffiliateFilterCollection")).GetAwaiter().GetResult())
            {
                _collectionDataFactory.GetCollectionDataHelper().DeleteCollection(ConfigurationManagerHelper.GetValuefromAppSettings("AffiliateFilterCollection")).Wait();
            }

            return _collectionDataFactory.GetCollectionDataHelper().CreateCollection(ConfigurationManagerHelper.GetValuefromAppSettings("AffiliateFilterCollection"), Constant.AffiliatePartitionKey).GetAwaiter().GetResult();
        }

        #endregion Implemented Methods
    }
}