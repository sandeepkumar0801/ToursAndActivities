using Isango.Entities;
using Isango.Entities.Bokun;
using Isango.Entities.ConsoleApplication.DataDumping;
using Isango.Entities.ElasticData;
using Isango.Entities.GlobalTixV3;
using Isango.Entities.Hotel;
using Isango.Entities.Master;
using Isango.Entities.Region;
using Isango.Entities.Rezdy;
using Isango.Entities.SiteMap;
using Isango.Entities.Tiqets;
using Isango.Entities.TourCMS;
using Isango.Entities.v1Css;
using Isango.Entities.Ventrata;
using cssbooking = ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities;
using System;
using System.Collections.Generic;
using GoldenTours = Isango.Entities.GoldenTours;
using TicketByRegion = Isango.Entities.TicketByRegion;
using ServiceAdapters.CitySightSeeing.CitySightSeeing.Entities;

namespace Isango.Persistence.Contract
{
    public interface IMasterPersistence
    {
        List<Currency> GetCurrencies(string affiliateId);

        Currency GetCurrency(string currencyCode);

        string GetCurrencyCodeForCountry(string countryCode);

        List<Region> GetCountries(string languageCode);

        List<LatLongVsurlMapping> GetRegionData();

        int GetRegionIdFromGeotree(int activityId);

        List<CrossSellLogic> GetCrossSellLogic();

        List<TicketByRegion> GetFilteredThemeparkTickets();

        Dictionary<string, string> GetSupportPhonesWithCountryCode(string affiliateId, string language);

        Dictionary<int, List<CrossSellProduct>> GetAllCrossSellProducts();

        Tuple<List<SiteMapData>, int> GetSiteMapData(string affiliateId, string languageCode = "en");

        Dictionary<string, string> LoadIndexedAttractionToRegionUrls();

        List<CurrencyExchangeRates> LoadExchangeRates();

        List<Language> GetSupportedLanguages();

        List<Facility> GetAllFacilities();

        List<MappedLanguage> LoadMappedLanguage();

        List<Entities.HotelBeds.MappedRegion> RegionVsDestination();

        List<RegionCategorySimilarProducts> GetRegionCategoryMapping();

        List<NetPriceMasterData> LoadNetPriceMasterData();

        List<IsangoHBProductMapping> LoadFactSheetMapping();

        List<AgeGroup> GetGliAgeGroupsByActivity();

        List<AgeGroup> GetPrioAgeGroupsByActivity();

        List<AgeGroup> GetAotAgeGroupsByActivity();

        List<CustomerPrototype> GetCustomerPrototypeByActivity();

        List<FareHarborAgeGroup> GetFareHarborAgeGroupsByActivity();

        List<FareHarborUserKey> GetAllFareHarborUserKeys();

        List<PriceCategory> GetBokunPriceCategoryByActivity();

        List<LocalizedDestinations> GetLocalizedDestinations();

        List<LocalizedCategories> GetLocalizedCategories();

        List<UrlPageIdMapping> UrlPageIdMappingList();

        List<HotelBedsCredentials> LoadHBauthData();

        List<PickupLocation> GetPickupLocationsByActivity();

        List<IsangoHBProductMapping> LoadLiveHBOptions();

        List<Entities.Ventrata.SupplierDetails> GetVentrataSupplierDetails();

        List<IsangoHBProductMapping> LoadLiveHBOptionsApiTudeContent();

        AffiliateAPI LoadDeltaAffiliate();

        List<LocalizedMerchandising> LoadDeltaAttractions();

        List<RegionCategoryMapping> LoadDeltaRegionAttraction();

        List<RegionSubAttraction> LoadDeltaRegionSubAttraction();

        string LoadMarketingCJFeed(int currencyid);

        List<TiqetsPaxMapping> LoadTiqetsPaxMappings();

        List<CJFeedProduct> LoadMarketingCriteoFeed(string currencycode);

        List<GoldenTours.PassengerMapping> GetPassengerMapping();

        List<GeoDetails> LoadDeltaGeoDetails();

        List<Entities.Destination> LoadDeltaDestination();

        List<ProductSupplier> LoadDeltaProductSupplier();

        List<Currency> LoadMasterCurrency();

        List<Language> LoadMasterLanguages();

        List<GeoDetails> LoadMasterGeoDetail(string language);

        List<RegionWiseProductDetails> LoadMasterRegionWise(string affiliateId, string categoryid = null);

        List<ServiceCancellationPolicy> GetServiceCancellationPolicies();

        List<RezdyLabelDetail> GetRezdyLabelDetails();

        List<RezdyPaxMapping> GetRezdyPaxMappings();

        List<TourCMSMapping> GetTourCMSPaxMappings();

        List<GlobalTixV3Mapping> GetGlobalTixV3PaxMappings();

        List<VentrataPaxMapping> GetVentrataPaxMappings();

        List<string> LoadAgeDumpngAPIs();

        Tuple<List<APIImages>, List<APIImages>> GetAPIImages();

        void SaveImagesUploadResult(List<ImagesUploadResult> imagesUploadResults, List<ImagesDeleteResult> imagesDeleteResults);

        List<TourCMSChannelList> LoadTourCMSSelectedChannel();

        List<ElasticDestinations> LoadElasticSearchDestinations();

        List<ElasticProducts> LoadElasticSearchProducts();

        List<ElasticAttractions> LoadElasticSearchAttractions();

        void SaveElasticDestination(List<DestinationDatum> datum);

        void SaveElasticProducts(List<ProductDatum> datum);

        void SaveElasticAttraction(List<AttractionDatum> datum);
        List<ElasticAffiliate> LoadElasticAffiliate();
        // List<AffiliateDetails> LoadAffiliateDetails();


        List<AgeGroup> GetPrioHubAgeGroupsByActivity();

        AvailablePersonTypes GetPersonTypeOptionCacheAvailability(int? activityId, int? serviceOptionId, DateTime? fromDate, DateTime? toDate);

        List<IsangoHBProductMapping> LoadLiveHBOptions(int? activityId, int? activityOptionId);

        int GetActivityIdByOptionId(int? activityOptionId);
        void SaveTokenAndRefIds(string tokenId, List<string> availablityRefereceIds);

        string GetTokenByAvailabilityReferenceId(string AvailabilityReferenceId);

        List<VentrataPackages> LoadVentrataPackages(string productid, string optionid);

        List<TiqetsPackage> LoadTiqetsPackages(string Product_ID);


        void SaveImageAltText(string ImageName, string AltText);

        void SaveAllCssExternalProducts(List<ExternalProducts> externalproducts);

        void SaveAllCssBooking(string IdempotancyKey, string Process, CssBookingDatas data, string status, string OTAReferenceId, string bookingRerenceNumber = null, bool IsCancelled = false, string cssrequest = null, string cssresponse = null, string barcode = null);
        List<string> IsBookingDoneWithCss(int CssProductOptionId = 0, string CssReferenceNumber = null);
        List<Entities.Booking.CssCancellation> GetCssCancellation();
        CssBookings GetBookingData();
        void SaveAllCssCancellation(string referenceNumber, string IdempotancyKey, string Process, int isangooptionid, string OTAReferenceId = null, bool IsCancelled = false, string cssrequest = null, string cssresponse = null);
        List<RedemptionRequest> GetRedemptionData();

        bool IsSqlServerHealthy();
    }
}