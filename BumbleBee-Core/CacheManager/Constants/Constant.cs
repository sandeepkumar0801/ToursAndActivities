using System;
using Util;

namespace CacheManager.Constants
{
    public sealed class Constant
    {
        public static int ParallelProcessorCount;

        static Constant()
        {
            try
            {
                double.TryParse(ConfigurationManagerHelper.GetValuefromAppSettings(AppConfigKey.ParallelProcessorCount), out double processorCountFromConfig);
                ParallelProcessorCount = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * processorCountFromConfig) * 1.0));
            }
            catch
            {
                ParallelProcessorCount = 1;
            }
        }

        public const string IsangoDb = "IsangoDB";
        public const string SyncTimeout = "SyncTimeout";
        public const string CosmosEndPointUrl = "CosmosEndPointURL";
        public const string CosmosPrimaryKey = "CosmosPrimaryKey";
        public const string PDFReactorURL = "PDFReactorURL";
        public const string CosmosDb = "database";
        public const string ActivityCollection = "ActivityCollection";
        public const string GetActivitiesQuery = "SELECT * FROM Activity";
        public const string GetSearchResultByProductIdQuery = "SELECT VALUE A FROM A Where A.activityId =";
        public const string GetSearchByKeywordQuery = "SELECT VALUE c FROM c Join a in c.Regions where CONTAINS(a.Name ,{0})";
        public const string MasterDataCollection = "MasterDataCollection";
        public const string AffiliateCollection = "AffiliateCollection";
        public const string PartitionKeyAffiliateCollection = "/id";
        public const string RegionTypeFilter = "city";
        public const int BadgeFilterId = 25;
        public const string Today = "today";
        public const string Tomorrow = "tomorrow";
        public const string Seven = "seven";
        public const int AddOneDay = 1;
        public const int AddSixDays = 6;
        public const int AddEightyNineDays = 89;
        public const string DefaultAffiliateId = "DefaultAffiliateID";
        public const string DefaultCurrencyCode = "DefaultCurrencyCode";
        public const string CurrencyExchangeRate = "CurrencyExchangeRate";
        public const string PartitionKeyActivityCollection = "/activityId";
        public const string PartitionKeyMasterCollection = "/id";
        public const string HbRegion = "HBRegion";
        public const string AffiliateExtendedLanguage = "AffiliateExtendedLanguageMapping";
        public const string UnityUrls = "UnityUrls";
        public const string RegionCategoryMappingProducts = "RegionCategoryMappingProducts";

        public const string ActivityUrlsCollection = "ActivityUrlsCollection";
        public const string HBProductMappingCollection = "HBProductMappingCollection";
        public const string RegionCategoryMappingCollection = "RegionCategoryMappingCollection";
        public const string AgeGroupByActivityCollection = "AgeGroupByActivityCollection";
        public const string NetPriceDataCollection = "NetPriceDataCollection";
        public const string HotelBedAvailabilityCollection = "HotelBedAvailabilityCollection";
        public const string AttractonActivityMappingCollection = "AttractonActivityMappingCollection";

        public const string HBProductMappingCollectionPartitionKey = "/ApiType";
        public const string NetPriceDataCollectionPartitionKey = "/AffiliateId";
        public const string RegionCategoryMappingCollectionPartitionKey = "/AttractionId";
        public const string ActivityUrlsCollectionPartitionKey = "/UrlType";
        public const string HotelBedAvailabilityCollectionPartitionKey = "/ServiceOptionId";
        public const string AttractonActivityMappingCollectionParitionKey = "/AttractionId";
        public const string ActivityIdPartitionKey = "/ActivityId";

        public const string PricingRulesCollection = "PricingRulesCollection";
        public const string ProductSaleRuleByActivity = "ProductSaleRuleByActivity";
        public const string ProductSaleRuleByOption = "ProductSaleRuleByOption";
        public const string ProductSaleRuleByAffiliate = "ProductSaleRuleByAffiliate";
        public const string ProductSaleRuleByCountry = "ProductSaleRuleByCountry";
        public const string ProductCostSaleRuleByActivity = "ProductCostSaleRuleByActivity";
        public const string ProductCostSaleRuleByOption = "ProductCostSaleRuleByOption";
        public const string ProductCostSaleRuleByAffiliate = "ProductCostSaleRuleByAffiliate";
        public const string ProductCostSaleRuleByCountry = "ProductCostSaleRuleByCountry";
        public const string B2BNetRateRules = "B2BNetRateRules";
        public const string B2BSaleRules = "B2BSaleRules";
        public const string SupplierSaleRuleByActivity = "SupplierSaleRuleByActivity";
        public const string SupplierSaleRuleByOption = "SupplierSaleRuleByOption";
        public const string GetPricingRules = "SELECT Value A FROM A where A.id = ";
        public const string PartitionKey = "/id";
        public const string FareHarborCustomerPrototype = "FareHarborCustomerPrototype";
        public const string FareHarborUserKey = "FareHarborUserKey";
        public const string LanguageCodeEnglish = "en";
        public const string AffiliatePartitionKey = "/affiliateId";
        public const string IsangoConfiguration = "IsangoConfigurationCollection";
        public const string PreExpirationTimeQuery = "Select * from c where c.id = 'PREExpirationTime'";
        public const string TiqetsPaxMapping = "TiqetsPaxMapping";
        public const string GoldenToursPaxMapping = "GoldenToursPaxMapping";
        public const string LocalCosmosEmulator = "LocalCosmosEmulator";
        public const string RezdyPaxMapping = "RezdyPaxMapping";
        public const string RezdyLabelDetails = "RezdyLabelDetails";
        //Error Messages Start
        public const string ActivityNotInCosmos = "Activity not found in Cosmos";
        //Error Messages End
        public const string TourCMSPaxMapping = "TourCMSPaxMapping";
        public const string VentrataPaxMapping = "VentrataPaxMapping";
        public const string GlobalTixV3Mapping = "GlobalTixV3PaxMapping";
    }

    public struct AppConfigKey
    {
        public const string ParallelProcessorCount = "MaxParallelThreadCount";
        public const string ExpirationCacheKeyTimeInMinutes = "CacheKeyExpirationTimeInMinutes";
        public const string IsEnableCaching = "IsEnableCaching";
    }
}