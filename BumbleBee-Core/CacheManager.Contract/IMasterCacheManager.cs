using Isango.Entities;
using Isango.Entities.GlobalTixV3;
using Isango.Entities.GoldenTours;
using Isango.Entities.Rezdy;
using Isango.Entities.TourCMS;
using Isango.Entities.Ventrata;
using Isango.Entities.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CacheManager.Contract
{
    public interface IMasterCacheManager
    {
        List<Currency> GetCurrencies(string affiliateId);

        bool InsertCurrencyInCache(Currency document);

        CacheKey<LatLongVsurlMapping> GetRegionData(string key);

        bool SetRegionData(CacheKey<LatLongVsurlMapping> latLongVSURLMapping);

        CacheKey<AutoSuggest> GetMasterAutoSuggestData(string key);

        bool SetMasterAutoSuggestData(CacheKey<AutoSuggest> autoSuggestList);

        CacheKey<BlogData> GetBlogData(string key);

        bool SetBlogData(CacheKey<BlogData> blogData);

        Task<bool> DeleteDocument(string collectionName, string id, int partitionKey);

        Task<bool> DeleteDocument(string collectionName, string id, string partitionKey);

        bool LoadCurrencyExchangeRate(CacheKey<CurrencyExchangeRates> currencyExchangeRate);

        bool SetFilteredTicketsToCache(CacheKey<TicketByRegion> ticketByRegion);

        CacheKey<TicketByRegion> GetFilteredTickets(string cacheKey);

        bool SetAutoSuggestData(CacheKey<AutoSuggest> autoSuggestData);

        CacheKey<AutoSuggest> GetAutoSuggestData(string affiliateId);

        List<AutoSuggest> GetAutoSuggestByAffiliateId(string affiliateId);

        List<CurrencyExchangeRates> GetCurrencyExchangeRate();

        bool SetHBAuthorizationData(CacheKey<HotelBedsCredentials> cacheKey);

        List<HotelBedsCredentials> GetHBAuthorizationData();

        bool LoadGoldenToursPassengerMappings(CacheKey<PassengerMapping> cacheResult);

        List<PassengerMapping> GetGoldenToursPaxMappings();

        bool LoadRezdyLabelDetailsMappings(CacheKey<RezdyLabelDetail> cacheResult);
        bool LoadRezdyPaxMappings(CacheKey<RezdyPaxMapping> cacheResult);
        List<RezdyPaxMapping> GetRezdysPaxMappings();
        List<RezdyLabelDetail> GetRezdyLabelDetails();

        bool LoadTourCMSPaxMappings(CacheKey<TourCMSMapping> cacheResult);
        bool LoadGlobalTixV3Mappings(CacheKey<GlobalTixV3Mapping> cacheResult);
        List<TourCMSMapping> GetTourCMSMappings();

        bool LoadVentrataPaxMappings(CacheKey<VentrataPaxMapping> cacheResult);
        List<VentrataPaxMapping> GetVentrataPaxMappings();
        List<GlobalTixV3Mapping> GetGlobalTixV3Mappings();

        Task<bool> DeleteOlderDocument(string collectionName, long timeStamp);
    }
}