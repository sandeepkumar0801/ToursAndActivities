using CacheManager.Constants;
using CacheManager.Contract;
using CacheManager.Helper;
using Isango.Entities;
using Isango.Entities.GlobalTixV3;
using Isango.Entities.GoldenTours;
using Isango.Entities.Rezdy;
using Isango.Entities.TourCMS;
using Isango.Entities.Ventrata;
using Isango.Entities.Wrapper;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using Util;

namespace CacheManager
{
    public class MasterCacheManager : IMasterCacheManager
    {
        #region Variables
        private readonly CollectionDataFactory<Currency> _collectionDataFactory;
        private readonly CollectionDataFactory<CacheKey<LatLongVsurlMapping>> _collectionDataFactoryForLatLongVsurlMapping;
        private readonly CollectionDataFactory<CacheKey<AutoSuggest>> _collectionDataFactoryForAutoSuggest;
        private readonly CollectionDataFactory<CacheKey<BlogData>> _collectionDataFactoryForBlogData;
        private readonly CollectionDataFactory<CacheKey<CurrencyExchangeRates>> _collectionDataFactoryCurrencyExchangeRates;
        private readonly CollectionDataFactory<CacheKey<TicketByRegion>> _collectionDataFactoryTickets;
        private readonly CollectionDataFactory<CacheKey<AutoSuggest>> _collectionDataFactoryAutoSuggest;
        private readonly CollectionDataFactory<CacheKey<HotelBedsCredentials>> _collectionDataFactoryForHotelBedsCredentials;
        private readonly CollectionDataFactory<CacheKey<PassengerMapping>> _collectionDataFactoryForPassengerMapping;
        private readonly CollectionDataFactory<CacheKey<RezdyLabelDetail>> _collectionDataFactoryForRezdyLabelDetails;
        private readonly CollectionDataFactory<CacheKey<RezdyPaxMapping>> _collectionDataFactoryForRezdyPaxMapping;
        private readonly CollectionDataFactory<CacheKey<TourCMSMapping>> _collectionDataFactoryForTourCMSPaxMapping;
        private readonly CollectionDataFactory<CacheKey<GlobalTixV3Mapping>> _collectionDataFactoryForGlobalTixV3Mapping;
        private readonly CollectionDataFactory<CacheKey<VentrataPaxMapping>> _collectionDataFactoryForVentrataPaxMapping;
        #endregion Variables

        #region Constructor

        public MasterCacheManager(CollectionDataFactory<Currency> collectionDataFactory,
            CollectionDataFactory<CacheKey<LatLongVsurlMapping>> collectionDataFactoryForLatLongVsurlMapping,
            CollectionDataFactory<CacheKey<AutoSuggest>> collectionDataFactoryForAutoSuggest,
            CollectionDataFactory<CacheKey<BlogData>> collectionDataFactoryForBlogData,
            CollectionDataFactory<CacheKey<CurrencyExchangeRates>> collectionDataFactoryCurrencyExchangeRates,
            CollectionDataFactory<CacheKey<TicketByRegion>> collectionDataFactoryTickets,
            CollectionDataFactory<CacheKey<AutoSuggest>> collectionDataFactoryAutoSuggest,
            CollectionDataFactory<CacheKey<HotelBedsCredentials>> collectionDataFactoryForHotelBedsCredentials,
            CollectionDataFactory<CacheKey<PassengerMapping>> collectionDataFactoryForPassengerMapping,
            CollectionDataFactory<CacheKey<RezdyLabelDetail>> collectionDataFactoryForRezdyLabelDetails = null,
            CollectionDataFactory<CacheKey<RezdyPaxMapping>> collectionDataFactoryForRezdyPaxMapping = null,
            CollectionDataFactory<CacheKey<TourCMSMapping>> collectionDataFactoryForTourCMSPaxMapping = null,
            CollectionDataFactory<CacheKey<VentrataPaxMapping>> collectionDataFactoryForVentrataPaxMapping = null,

            CollectionDataFactory<CacheKey<GlobalTixV3Mapping>> collectionDataFactoryForGlobalTixV3Mapping = null)
        {
            _collectionDataFactory = collectionDataFactory;
            _collectionDataFactoryForLatLongVsurlMapping = collectionDataFactoryForLatLongVsurlMapping;
            _collectionDataFactoryForAutoSuggest = collectionDataFactoryForAutoSuggest;
            _collectionDataFactoryForBlogData = collectionDataFactoryForBlogData;
            _collectionDataFactoryCurrencyExchangeRates = collectionDataFactoryCurrencyExchangeRates;
            _collectionDataFactoryTickets = collectionDataFactoryTickets;
            _collectionDataFactoryAutoSuggest = collectionDataFactoryAutoSuggest;
            _collectionDataFactoryForHotelBedsCredentials = collectionDataFactoryForHotelBedsCredentials;
            _collectionDataFactoryForPassengerMapping = collectionDataFactoryForPassengerMapping;
            _collectionDataFactoryForRezdyLabelDetails = collectionDataFactoryForRezdyLabelDetails;
            _collectionDataFactoryForRezdyPaxMapping = collectionDataFactoryForRezdyPaxMapping;
            _collectionDataFactoryForTourCMSPaxMapping = collectionDataFactoryForTourCMSPaxMapping;
            _collectionDataFactoryForVentrataPaxMapping = collectionDataFactoryForVentrataPaxMapping;
            _collectionDataFactoryForGlobalTixV3Mapping = collectionDataFactoryForGlobalTixV3Mapping;
        }

        #endregion Constructor

        #region GetCurrencies

        /// <summary>
        /// This Method with return the list of currencies related to the affiliateID.
        /// </summary>
        /// <param name="affiliateId"></param>
        /// <returns> List of Currency</returns>
        public List<Currency> GetCurrencies(string affiliateId)
        {
            var query = $"select * from {ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection")} M where M.CacheKey = '{affiliateId}'";
            //for mongoDB
            var filter = Builders<Currency>.Filter.Eq("CacheKey", affiliateId);
            //end mongo
            var result = _collectionDataFactory.GetCollectionDataHelper().GetResultList(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), query, filter);
            return result;
        }

        #endregion GetCurrencies

        #region InsertCurrencyInCache

        /// <summary>
        /// Insert currency List into the cache (Cosmos DB)
        /// </summary>
        /// <param name="document"></param>
        /// <returns> true / false</returns>
        public bool InsertCurrencyInCache(Currency document)
        {
            CheckIfMasterDataCollectionExists(_collectionDataFactory);
            var result = _collectionDataFactory.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), document).Result;
            return result;
        }

        #endregion InsertCurrencyInCache

        #region GetRegion

        public CacheKey<LatLongVsurlMapping> GetRegionData(string key)
        {
            var query = $"select VALUE M from {ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection")} M where M.id='{key}'";
            //for mongoDB
            var filter = Builders<CacheKey<LatLongVsurlMapping>>.Filter.Eq("_id", key);
            //end mongo
            var result = _collectionDataFactoryForLatLongVsurlMapping.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), query, filter);
            return result;
        }

        public bool SetRegionData(CacheKey<LatLongVsurlMapping> latLongVSURLMapping)
        {
            CheckIfMasterDataCollectionExists(_collectionDataFactory);
            if (_collectionDataFactoryForLatLongVsurlMapping.GetCollectionDataHelper().CheckIfDocumentExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), latLongVSURLMapping.Id).Result)
            {
                return _collectionDataFactoryForLatLongVsurlMapping.GetCollectionDataHelper().UpdateDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), latLongVSURLMapping).Result;
            }
            else
            {
                return _collectionDataFactoryForLatLongVsurlMapping.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), latLongVSURLMapping).Result;
            }
        }

        #endregion GetRegion

        public CacheKey<AutoSuggest> GetMasterAutoSuggestData(string key)
        {
            var query = $"select VALUE M from {ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection")} M where M.id='{key}'";
            //for mongoDB
            var filter = Builders<CacheKey<AutoSuggest>>.Filter.Eq("_id", key);
            //end mongo
            var result = _collectionDataFactoryForAutoSuggest.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), query, filter);
            return result;
        }

        public bool SetMasterAutoSuggestData(CacheKey<AutoSuggest> autoSuggestList)
        {
            CheckIfMasterDataCollectionExists(_collectionDataFactory);

            if (_collectionDataFactoryForAutoSuggest.GetCollectionDataHelper().CheckIfDocumentExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), autoSuggestList.Id).Result)
            {
                return _collectionDataFactoryForAutoSuggest.GetCollectionDataHelper().UpdateDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), autoSuggestList).Result;
            }
            else
            {
                return _collectionDataFactoryForAutoSuggest.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), autoSuggestList).Result;
            }
        }

        public CacheKey<BlogData> GetBlogData(string key)
        {
            var query = $"select VALUE M from {ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection")} M where M.id='{key}'";
            //for mongoDB
            var filter = Builders<CacheKey<BlogData>>.Filter.Eq("_id", key);
            //end mongo
            var result = _collectionDataFactoryForBlogData.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), query, filter);
            return result;
        }

        public bool SetBlogData(CacheKey<BlogData> blogData)
        {
            CheckIfMasterDataCollectionExists(_collectionDataFactory);

            if (_collectionDataFactoryForBlogData.GetCollectionDataHelper().CheckIfDocumentExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), blogData.Id).Result)
            {
                return _collectionDataFactoryForBlogData.GetCollectionDataHelper().UpdateDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), blogData).Result;
            }
            else
            {
                var result = _collectionDataFactoryForBlogData.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), blogData);
                result.Wait();
                return result.Result;
            }
        }

        public bool LoadCurrencyExchangeRate(CacheKey<CurrencyExchangeRates> currencyExchangeRate)
        {
            CheckIfMasterDataCollectionExists(_collectionDataFactory);

            if (_collectionDataFactoryCurrencyExchangeRates.GetCollectionDataHelper().CheckIfDocumentExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), currencyExchangeRate.Id).Result)
            {
                return _collectionDataFactoryCurrencyExchangeRates.GetCollectionDataHelper().UpdateDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), currencyExchangeRate).Result;
            }
            else
            {
                return _collectionDataFactoryCurrencyExchangeRates.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), currencyExchangeRate).Result;
            }
        }

        public List<CurrencyExchangeRates> GetCurrencyExchangeRate()
        {
            var query = $"select VALUE M from {ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection")} M where M.id='CurrencyExchangeRate'";
            //for mongoDB
            var filter = Builders<CacheKey<CurrencyExchangeRates>>.Filter.Eq("_id", "CurrencyExchangeRate");
            //end mongo
            var result = _collectionDataFactoryCurrencyExchangeRates.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), query, filter);
            return result.CacheValue;
        }

        public Task<bool> DeleteDocument(string collectionName, string id, string partitionKey)
        {
            CheckIfMasterDataCollectionExists(_collectionDataFactory);
            var result = _collectionDataFactory.GetCollectionDataHelper().DeleteDocument(collectionName, id, partitionKey);
            result.Wait();
            return result;
        }

        public Task<bool> DeleteDocument(string collectionName, string id, int partitionKey)
        {
            CheckIfMasterDataCollectionExists(_collectionDataFactory);
            var result = _collectionDataFactory.GetCollectionDataHelper().DeleteDocument(collectionName, id, partitionKey);
            result.Wait();
            return result;
        }

        public Task<bool> DeleteOlderDocument(string collectionName, long timeStamp)
        {
            //CheckIfMasterDataCollectionExists(_collectionDataFactory);
            var result = _collectionDataFactory.GetCollectionDataHelper().DeleteOlderDocuments(collectionName, timeStamp);
            result.Wait();
            return result;
        }

        public bool SetFilteredTicketsToCache(CacheKey<TicketByRegion> ticketByRegion)
        {
            CheckIfMasterDataCollectionExists(_collectionDataFactory);
            var result = _collectionDataFactoryTickets.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), ticketByRegion);
            result.Wait();
            return result.Result;
        }

        public CacheKey<TicketByRegion> GetFilteredTickets(string cacheKey)
        {
            var query = $"select VALUE M from {ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection")} M where M.id='{cacheKey}'";
            //for mongoDB
            var filter = Builders<CacheKey<TicketByRegion>>.Filter.Eq("_id", cacheKey);
            //end mongo
            return _collectionDataFactoryTickets.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), query, filter);
        }

        public List<AutoSuggest> GetAutoSuggestByAffiliateId(string affiliateId)
        {
            var result = GetAutoSuggestData(affiliateId);
            return result != null ? GetAutoSuggestData(affiliateId).CacheValue : null;
        }

        public bool SetAutoSuggestData(CacheKey<AutoSuggest> autoSuggestData)
        {
            CheckIfMasterDataCollectionExists(_collectionDataFactory);

            return _collectionDataFactoryAutoSuggest.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), autoSuggestData).Result;
        }

        public CacheKey<AutoSuggest> GetAutoSuggestData(string affiliateId)
        {
            var key = $"AutoSuggestData {affiliateId}";
            var query = $"select VALUE M from {ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection")} M where M.id= '{key}'";
            //for mongoDB
            var filter = Builders<CacheKey<AutoSuggest>>.Filter.Eq("_id", key);
            //end mongo
            var result = _collectionDataFactoryAutoSuggest.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), query, filter);
            return result;
        }

        public bool SetHBAuthorizationData(CacheKey<HotelBedsCredentials> cacheKey)
        {
            CheckIfMasterDataCollectionExists(_collectionDataFactory);

            if (_collectionDataFactoryForHotelBedsCredentials.GetCollectionDataHelper().CheckIfDocumentExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheKey.Id).Result)
            {
                return _collectionDataFactoryForHotelBedsCredentials.GetCollectionDataHelper().UpdateDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheKey).Result;
            }
            else
            {
                var result = _collectionDataFactoryForHotelBedsCredentials.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheKey);
                result.Wait();
                return result.Result;
            }
        }

        public List<HotelBedsCredentials> GetHBAuthorizationData()
        {
            var collectionName = ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection");
            //for mongoDB
            var filter = Builders<CacheKey<HotelBedsCredentials>>.Filter.Eq("_id", "HBAuthorizationData");
            //end mongo
            var query = $"select VALUE M from {collectionName} M where M.id='HBAuthorizationData'";
            return _collectionDataFactoryForHotelBedsCredentials.GetCollectionDataHelper().GetResult(collectionName, query, filter).CacheValue;
        }

        public bool LoadGoldenToursPassengerMappings(CacheKey<PassengerMapping> cacheResult)
        {
            if (!_collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection")).Result)
            {
                _collectionDataFactory.GetCollectionDataHelper().CreateCollection(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), Constant.PartitionKeyMasterCollection);
            }

            if (_collectionDataFactory.GetCollectionDataHelper().CheckIfDocumentExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheResult.Id).Result)
            {
                return _collectionDataFactoryForPassengerMapping.GetCollectionDataHelper().UpdateDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheResult).Result;
            }
            else
            {
                return _collectionDataFactoryForPassengerMapping.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheResult).Result;
            }
        }

        public List<PassengerMapping> GetGoldenToursPaxMappings()
        {
            //for mongoDB
            var filter = Builders<CacheKey<PassengerMapping>>.Filter.Eq("_id", "GoldenToursPaxMapping");
            //end mongo
            var cache = _collectionDataFactoryForPassengerMapping.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), $"select * from c where c.id='{Constant.GoldenToursPaxMapping}'", filter);
            var result = cache?.CacheValue;

            return result;
        }

        private void CheckIfMasterDataCollectionExists(CollectionDataFactory<Currency> _collectionDataFactory)
        {
            if (!_collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection")).Result)
            {
                _collectionDataFactory.GetCollectionDataHelper().CreateCollection(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), Constant.PartitionKeyMasterCollection);
            }
        }

        #region [Rezdy]
        public bool LoadRezdyLabelDetailsMappings(CacheKey<RezdyLabelDetail> cacheResult)
        {
            if (!_collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection")).Result)
            {
                _collectionDataFactory.GetCollectionDataHelper().CreateCollection(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), Constant.PartitionKeyMasterCollection);
            }


            if (_collectionDataFactoryForRezdyLabelDetails.GetCollectionDataHelper().CheckIfDocumentExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheResult.Id).Result)
            {
                return _collectionDataFactoryForRezdyLabelDetails.GetCollectionDataHelper().UpdateDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheResult).Result;
            }
            else
            {
                return _collectionDataFactoryForRezdyLabelDetails.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheResult).Result;
            }
        }

        public bool LoadRezdyPaxMappings(CacheKey<RezdyPaxMapping> cacheResult)
        {
            if (!_collectionDataFactory.GetCollectionDataHelper().CheckIfCollectionExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection")).Result)
            {
                _collectionDataFactory.GetCollectionDataHelper().CreateCollection(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), Constant.PartitionKeyMasterCollection);
            }

            if (_collectionDataFactoryForRezdyPaxMapping.GetCollectionDataHelper().CheckIfDocumentExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheResult.Id).Result)
            {
                return _collectionDataFactoryForRezdyPaxMapping.GetCollectionDataHelper().UpdateDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheResult).Result;
            }
            else
            {
                return _collectionDataFactoryForRezdyPaxMapping.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheResult).Result;
            }
        }

        public bool LoadTourCMSPaxMappings(CacheKey<TourCMSMapping> cacheResult)
        {

            if (!_collectionDataFactoryForTourCMSPaxMapping.GetCollectionDataHelper().CheckIfCollectionExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection")).Result)
            {
                _collectionDataFactoryForTourCMSPaxMapping.GetCollectionDataHelper().CreateCollection(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), Constant.PartitionKeyMasterCollection);
            }

            if (_collectionDataFactoryForTourCMSPaxMapping.GetCollectionDataHelper().CheckIfDocumentExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheResult.Id).Result)
            {
                return _collectionDataFactoryForTourCMSPaxMapping.GetCollectionDataHelper().UpdateDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheResult).Result;
            }
            else
            {
                return _collectionDataFactoryForTourCMSPaxMapping.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheResult).Result;
            }
        }

        public bool LoadVentrataPaxMappings(CacheKey<VentrataPaxMapping> cacheResult)
        {

            if (!_collectionDataFactoryForVentrataPaxMapping.GetCollectionDataHelper().CheckIfCollectionExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection")).Result)
            {
                _collectionDataFactoryForVentrataPaxMapping.GetCollectionDataHelper().CreateCollection(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), Constant.PartitionKeyMasterCollection);
            }

            if (_collectionDataFactoryForVentrataPaxMapping.GetCollectionDataHelper().CheckIfDocumentExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheResult.Id).Result)
            {
                return _collectionDataFactoryForVentrataPaxMapping.GetCollectionDataHelper().UpdateDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheResult).Result;
            }
            else
            {
                return _collectionDataFactoryForVentrataPaxMapping.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheResult).Result;
            }
        }



        public bool LoadGlobalTixV3Mappings(CacheKey<GlobalTixV3Mapping> cacheResult)
        {

            if (!_collectionDataFactoryForGlobalTixV3Mapping.GetCollectionDataHelper().CheckIfCollectionExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection")).Result)
            {
                _collectionDataFactoryForGlobalTixV3Mapping.GetCollectionDataHelper().CreateCollection(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), Constant.PartitionKeyMasterCollection);
            }

            if (_collectionDataFactoryForGlobalTixV3Mapping.GetCollectionDataHelper().CheckIfDocumentExist(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheResult.Id).Result)
            {
                return _collectionDataFactoryForGlobalTixV3Mapping.GetCollectionDataHelper().UpdateDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheResult).Result;
            }
            else
            {
                return _collectionDataFactoryForGlobalTixV3Mapping.GetCollectionDataHelper().InsertDocument(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), cacheResult).Result;
            }
        }
        public List<RezdyPaxMapping> GetRezdysPaxMappings()
        {
            var query = $"select * from c where c.id='{Constant.RezdyPaxMapping}'";
            //for mongoDB
            var filter = Builders<CacheKey<RezdyPaxMapping>>.Filter.Eq("_id", Constant.RezdyPaxMapping);
            //end mongo
            var cache = _collectionDataFactoryForRezdyPaxMapping.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), query, filter);
            var result = cache?.CacheValue;

            return result;
        }

        public List<TourCMSMapping> GetTourCMSMappings()
        {
            var query = $"select * from c where c.id='{Constant.TourCMSPaxMapping}'";
            //for mongoDB
            var filter = Builders<CacheKey<TourCMSMapping>>.Filter.Eq("_id", Constant.TourCMSPaxMapping);
            //end mongo
            var cache = _collectionDataFactoryForTourCMSPaxMapping.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), query, filter);
            var result = cache?.CacheValue;

            return result;

        }

        public List<VentrataPaxMapping> GetVentrataPaxMappings()
        {
            var query = $"select * from c where c.id='{Constant.VentrataPaxMapping}'";
            //for mongoDB
            var filter = Builders<CacheKey<VentrataPaxMapping>>.Filter.Eq("_id", Constant.VentrataPaxMapping);
            //end mongo
            var cache = _collectionDataFactoryForVentrataPaxMapping.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), query, filter);
            var result = cache?.CacheValue;

            return result;

        }

        public List<RezdyLabelDetail> GetRezdyLabelDetails()
        {
            var query = $"select * from c where c.id='{Constant.RezdyLabelDetails}'";
            //for mongoDB
            var filter = Builders<CacheKey<RezdyLabelDetail>>.Filter.Eq("_id", Constant.RezdyLabelDetails);
            //end mongo
            var cache = _collectionDataFactoryForRezdyLabelDetails.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), query, filter);
            var result = cache?.CacheValue;

            return result;
        }
        public List<GlobalTixV3Mapping> GetGlobalTixV3Mappings()
        {
            var query = $"select * from c where c.id='{Constant.GlobalTixV3Mapping}'";
            //for mongoDB
            var filter = Builders<CacheKey<GlobalTixV3Mapping>>.Filter.Eq("_id", Constant.GlobalTixV3Mapping);
            //end mongo
            var cache = _collectionDataFactoryForGlobalTixV3Mapping.GetCollectionDataHelper().GetResult(ConfigurationManagerHelper.GetValuefromAppSettings("MasterDataCollection"), query, filter);
            var result = cache?.CacheValue;

            return result;

        }
        #endregion
    }
}