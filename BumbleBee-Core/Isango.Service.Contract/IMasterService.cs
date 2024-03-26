using Isango.Entities;
using Isango.Entities.AdyenPayment;
using Isango.Entities.Bokun;
using Isango.Entities.Enums;
using Isango.Entities.Master;
using Isango.Entities.Region;
using Isango.Entities.SiteMap;
using Isango.Entities.v1Css;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoldenTours = Isango.Entities.GoldenTours;
using TicketByRegion = Isango.Entities.TicketByRegion;

namespace Isango.Service.Contract
{
    public interface IMasterService
    {
        Task<List<Currency>> GetCurrenciesAsync(string affiliateId);

        Task<Currency> GetCurrencyAsync(string currencyCode);

        Task<string> GetCurrencyCodeForCountryAsync(string countryCode);

        Task<List<Region>> GetCountriesAsync(string languageCode);

        Task<List<LatLongVsurlMapping>> GetRegionAsync();

        Task<int> GetRegionIdFromGeotreeAsync(int activityId);

        Task<List<TicketByRegion>> GetFilteredThemeparkTicketsAsync();

        Task<Dictionary<string, string>> GetSupportPhonesWithCountryCodeAsync(string affiliateId, string language);

        Task<Dictionary<int, List<CrossSellProduct>>> GetAllCrossSellProductsAsync();

        List<CrossSellLogic> GetCrossSellLogic();

        //Task<List<Activity>> GetCrossSellProducts(int? lineOfBusinessId);

        Task<Tuple<List<SiteMapData>, int>> GetSiteMapDataAsync(string affiliateId, string languageCode);

        Task<Dictionary<string, string>> LoadIndexedAttractionToRegionUrlsAsync();

        Task<List<Language>> GetSupportedLanguagesAsync();

        Task<List<CurrencyExchangeRates>> LoadCurrencyExchangeRatesAsync();

        Task<List<Entities.HotelBeds.MappedRegion>> LoadRegionVsDestinationAsync();

        Task<List<MappedLanguage>> LoadMappedLanguageAsync();

        Task<decimal> GetConversionFactorAsync(string baseCurrencyCode, string currencyCode);

        Task<List<LocalizedDestinations>> GetLocalizedDestinationsAsync();

        Task<List<LocalizedCategories>> GetLocalizedCategoriesAsync();

        Task<List<AgeGroup>> GetGLIAgeGroupAsync(int activityId, APIType apiType);

        Task<List<NetPriceMasterData>> LoadNetPriceMasterDataAsync();

        Task<List<PriceCategory>> GetBokunPriceCategoryByActivityAsync();

        Task<List<HotelBedsCredentials>> GetHBAuthorizationDataAsync();

        Task<List<PickupLocation>> GetPickupLocationsByActivityAsync(int activityId);

        Task<AffiliateAPI> GetDeltaAffiliateAsync();

        Task<List<LocalizedMerchandising>> GetDeltaAttractionsAsync();

        Task<List<RegionCategoryMapping>> GetDeltaRegionAttractionAsync();

        Task<List<RegionSubAttraction>> GetDeltaRegionSubAttractionAsync();

        Task<string> GetMarketingCJFeedAsync(int currencyid);

        Task<List<CJFeedProduct>> GetMarketingCriteoFeedAsync(string currencycode);

        Task<List<AgeGroup>> GetPrioAgeGroupAsync(int activityId, APIType apiType);

        Task<List<GoldenTours.PassengerMapping>> GetPassengerMapping(APIType apiType);

        Task<List<GeoDetails>> GetGeoDetailAsync();

        Task<List<Destination>> GetDestinationAsync();

        Task<List<ProductSupplier>> GetProductSupplierAsync();

        Task<List<Currency>> GetCurrencyAsync();

        Task<List<Language>> GetMasterLanguagesAsync();

        Task<List<GeoDetails>> GetMasterGeoDetailAsync(string language);

        Task<List<RegionWiseProductDetails>> GetMasterRegionWiseAsync(string affiliateId, string categoryid = null, string B2B = null);

        Task<List<Entities.Rezdy.RezdyLabelDetail>> GetLabelDetailsAsync();

        Task<List<Entities.Rezdy.RezdyPaxMapping>> GetPaxMappingsAsync();

        Task<PaymentMethodsResponse> GetAdyenPaymentMethodsAsync(string countryCode
            , string shopperLocale, string amount, string currency);

        Task<string> GetAdyenPaymentMethodsV2Async(string countryCode
            , string shopperLocale, string amount, string currency);

        List<Entities.Ventrata.SupplierDetails> GetVentrataData();

        Task<List<AgeGroup>> GetPrioHubAgeGroupAsync(int activityId, APIType apiType);

        AvailablePersonTypes GetPersonTypeOptionCacheAvailability(int? activityId = null, int? serviceOptionId = null, DateTime? fromDate = null, DateTime? toDate = null);

        void SaveImageAltText(string ImageName, string AltText);
    }
}