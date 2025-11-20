using System;
using Util;

namespace Isango.Service.Constants
{
    public sealed class Constant
    {
        public static int ParallelProcessorCount;

        static Constant()
        {
            double processorCountFromConfig = 0.5d;
            try
            {
                double.TryParse(ConfigurationManagerHelper.GetValuefromAppSettings("ParallelProcessorCount"), out processorCountFromConfig);
            }
            catch  // key not found in config
            {
                processorCountFromConfig = 0.5d;
            }
            try
            {
                if (processorCountFromConfig > 0.75 || processorCountFromConfig <= 0)
                {
                    processorCountFromConfig = 0.75;
                }

                ParallelProcessorCount = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * processorCountFromConfig) * 1.0));
            }
            catch
            {
                ParallelProcessorCount = 1;
            }
        }

        public const string StorageConnectionString = "StorageConnectionString";
        public const string MasterAutoSuggestDataKey = "MasterAutoSuggestData";
        public const string GeoCoordinateMasterMappingKey = "GeoCoordinateMasterMapping";
        public const string ExchangeRateBuffer = "exhangeRateBuffer";
        public const string BlogDataKey = "BlogDataForIsango";
        public const string DestinationCode = "D";
        public const string AttractionCode = "A";
        public const string AffiliateFilterCacheKey = "AffiliateFilter";
        public const string FilteredTickets = "FilteredTickets";
        public const int MaxDays = 89;
        public const int AddSixDays = 6;
        public const int AddFourteenDays = 15;
        public const string GetTicketAuthString = "ISANGOUK1013><ISANGOUK1013";
        public const string DateSeparator = "@";
        public const string GB = "GB";
        public const string IE = "IE";
        public const string CA = "CA";
        public const string AU = "AU";
        public const string US = "US";
        public const string ROW = "ROW";
        public const string Status = "subscribed";
        public const string IsangoAvailability = "IsangoAvailability";
        public const string DefaultAffiliateId = "DefaultAffiliateID";
        public const string DefaultCurrencyCode = "DefaultCurrencyCode";

        public const string AffiliateId_En = "5BEEF089-3E4E-4F0F-9FBF-99BF1F350183";
        public const string AffiliateId_De = "7FAAF455-1C75-4FC1-88DC-0C08F79F1439";
        public const string AffiliateId_Es = "B7951003-DA73-4AF9-A731-EF5064F6E343";
        public const string En = "en";
        public const string De = "de";
        public const string Es = "es";

        public const string CurrencyExchangeRate = "CurrencyExchangeRate";
        public const string City = "city";
        public const string AutoSuggestData = "AutoSuggestData";
        public const string RegionCategoryMapping = "RegionCategoryMapping";
        public const string RegionVsDestination = "RegionVsDestination";
        public const string MappedLanguage = "MappedLanguage";
        public const string InEnglish = "in english";
        public const string CustomerPrototypeCacheKey = "FareHarborCustomerPrototype";
        public const string AgeGroupCacheKey = "FareHarborAgeGroup";
        public const string FareHarborUserCacheKey = "FareHarborUserKey";
        public const string AlternateApiurl = "AlternateAPIURL";
        public const string AlternateGetAPIURL = "AlternateGetAPIURL";
        public const string AlternateAPIMode = "AlternateAPIMode";
        public const string Alternativepayment = "ALTERNATIVEPAYMENT";

        public const string Authentication = "EXAMPLE_AUTH_STRING";
        public const string HopOnHoffOffBus = "www.example-tours.com";
        public const string LocalGranCanariaTour = "www.example-canaria-tours.com";
        public const string AlhambraGranadaTour = "www.example-granada-tours.com";
        public const string LocalDubaiTours = "www.example-dubai-tours.com";
        public const string LocalVeniceTours = "www.example-venice-tours.com";
        public const string LocalParisTours = "www.example-paris-tours.com";
        public const string AlternateAPIKeyHOHO = "AlternateAPIKeyHOHO";
        public const string AlternateAPIKeyLGCT = "AlternateAPIKeyLGCT";
        public const string AlternateAPIKeyAGT = "AlternateAPIKeyAGT";
        public const string AlternateAPIKeyLDT = "AlternateAPIKeyLDT";
        public const string AlternateAPIKeyLVT = "AlternateAPIKeyLVT";
        public const string AlternateAPIKeyLPT = "AlternateAPIKeyLPT";
        public const string AlternateAPIKey = "AlternateAPIKey";

        public const string UrlPageIdMapping = "UrlPageIdMapping";
        public const string AutoCompleteHotel = "AutoCompleteHotel";
        public const string HbAuthorizationData = "HBAuthorizationData";

        public const string ServiceId = "serviceid";
        public const string ServiceOptionId = "serviceoptionid";
        public const string StartDate = "startDate";
        public const string EndDate = "endDate";
        public const string RegionId = "regionid";
        public const string CurrencyISOCode = "vcurrencyISOCode";
        public const string GatePrice = "GatePrice";
        public const string CostPrice = "CostPrice";

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
        public const string Enabled3D2DCountries = "3DAnd2DEnabledCountries";
        public const string Error = "Error";
        public const string ErrorCode524 = "Error524";
        public const string ErrorCode523 = "Error523";
        public const string ErrorCode522 = "Error522";
        public const string ErrorCode539 = "Error539";
        public const string PreDefinedErrorCodes = "PreDefinedErrorCodes";
        public const string EnrollStatusNotEnrolled = "NotEnrolled";
        public const string EnrollStatusInEligible = "Ineligible";
        public const string EnrollStatusFailure = "Failure";
        public const string StatusFailed = "Failed";
        public const string StatusSuccess = "Success";
        public const string StatusReservation = "Reservation";
        public const string StatusBooking = "CreateBooking";
        public const string StatusConfirmTicket = "ConfirmTicket";
        public const int Number705 = 705;

        public const string ResponseErrorNo =
            "WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_ENROLLMENT_CHECK/CC_TRANSACTION/PROCESSING_STATUS/ERROR/Number";

        public const string ResponseError =
            "WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_ENROLLMENT_CHECK/CC_TRANSACTION/PROCESSING_STATUS/ERROR";

        public const string BookingGuidString =
            "WIRECARD_BXML/W_RESPONSE/W_JOB/FNC_CC_ENROLLMENT_CHECK/CC_TRANSACTION/PROCESSING_STATUS/GuWID";

        public const int Number245 = 245;
        public const int Number145 = 145;
        public const int Number129 = 129;
        public const int Number628 = 628;
        public const string AmericanExpress = "American Express";
        public const string EnrollStatusFailureAmex = "Failure - Amex";

        public const string CountryTypeAustralia = "CountryTypeAustralia";
        public const string CountryTypeNewZealand = "CountryTypeNewZealand";
        public const string CountryTypeFiji = "CountryTypeFiji";

        public const string Isk = "ISK";
        public const string Days4Loop = "Days4Loop";
        public const string Days4LoopForLightData = "Days4LoopForLightData";
        public const string Days4LoopTiqets = "Days4LoopTiqets";
        public const string Days2FetchForHeavyData = "Days2FetchForHeavyData";

        public const string Days2FetchForLightData = "Days2FetchForLightData";

        public const string Days2FetchForTiqetsHeavyData = "Days2FetchForTiqetsHeavyData";
        public const string Days2FetchForApiTudeData = "Days2FetchForApiTudeData";
        public const string APiTudeCalendarRecordsAtTimeData = "APiTudeCalendarRecordsAtTimeData";
        public const string APiTudeContentRecordsAtTimeData = "ApiTudeContentRecordsAtTimeData";
        public const string TiqetsImageType = "image/jpeg";
        public const string TiqetsCheckinAddDays = "TiqetsCheckinAddDays";
        public const string TiqetsCheckOutAddDays = "TiqetsCheckOutAddDays";


        public const string Months2FetchForHeavyData = "Months2FetchForHeavyData";
        public const string Months2FetchForLightData = "Months2FetchForLightData";

        public const string PrioDistributorId = "PrioDistributorId";
        public const string PrioTokenKey = "PrioTokenKey";
        public const string DateFormat = "yyyy-MM-dd";

        public const string AuthKey = "ISANGOUK1013><ISANGOUK1013";
        public const string English = "ENG";

        public const string DuplicateBookingMessage = "Duplicate Booking";
        public const string Sofort = "sofort";
        public const string SafetyPay = "safetypay";
        public const string FailedBookingMessage = "Booking Failed";
        public const string InvalidPrepaidBooking = "Invalid PrepaidBooking";
        public const string InvalidPrepaid = "invalidprepaid";
        public const string SuccessBookingMessage = "Booking Successful";
        public const string StatusSuccessFull = "SUCCESFUL";
        public const string FailedStatus = "FAILED";
        public const string SuccessStatus = "SUCCESS";
        public const string WebhookTransactionCompleted = "WEBHOOKTRANSACTIONCOMPLETED";
        public const string WebhookResponseFailed = "WEBHOOKRESPONSEFAILED";
        public const string WebhookResponseAfterGetDeclined = "WEBHOOKRESPONSEAFTERGET_DECLINED";
        public const string StatusDeclined = "DECLINED";
        public const string Chargeback = "CHARGEBACK";
        public const string WebhookResponseChargeBack = "WEBHOOKRESPONSECHARGEBACK";
        public const string AlternartiveSuccessUrl = "AlternartiveSuccessUrl";
        public const string AlternartiveFailureUrl = "AlternartiveFailureUrl";
        public const string TermUrl = "TermUrl";
        public const string TransactionCompleted = "TRANSACTIONCOMPLETED";
        public const string BookingFailedInTransactionSuccess = "BOOKINGFAILEDINTRANSACTIONSUCCESS";
        public const string TransactionFailed = "TRANSACTIONFAILED";
        public const string WebHookResponse = "WEBHOOKRESPONSE";
        public const string WebHookResponseAfterGetTrans = "WEBHOOKRESPONSEAFTERGETTRANS";
        public const string WebhookException = "WEBHOOKEXCEPTION";
        public const string BookingConfirmedFromDb = "BOOKINGCONFIRMEDFROMDB";
        public const string WebhookResponseSuccessDb = "WEBHOOKRESPONSSUCCESSDB";

        public const string AdultFirstName = "AdultFirstName";
        public const string AdultLastName = "AdultLastName";
        public const string ChildFirstName = "ChildFirstName";
        public const string ChildLastName = "ChildLastName";
        public const int AdultAge = 30;
        public const int ChildAge = 15;
        public const string String = "STRING";
        public const string TicketPdfType = "Link";
        public const string Contract = "CONTRACT";
        public const string PreExpirationId = "PREExpirationTime";

        public const string DefaultLanguage = "en";
        public const string TiqetsPaxMapping = "TiqetsPaxMapping";
        public const string CountryIdAustralia = "6169";
        public const string CountryIdNewZealand = "6170";
        public const string CountryIdFiji = "8213";
        public const string ZeroTime = "00:00";

        public const string CreateOrder = "CreateOrder";
        public const string ConfirmOrder = "ConfirmOrder";
        public const string GetTicket = "GetTicket";

        public const string APIBookingNotAllowed = "Booking not allowed for Api--> {0} --> ServiceId {1}.";
        public const string ErrorMail = "ErrorMail";
        public const string GoldenToursPaxMapping = "GoldenToursPaxMapping";

        public const string CaptureReference = "ISANGO0001";
        public const string ApexxCapture = "ApexxCapture";
        public const string TicketPerPassenger = "TicketPerPassenger";

        public const string CreateBooking = "CreateBooking";
        public const string CreateReservation = "CreateReservation";
        public const string Reservation = "Reservation";
        public const string IsHBApiCallParallel = "IsHBApiCallParallel";

        public const string ExtraDetails = "ExtraDetails";
        public const string CancellationPolicy = "CancellationPolicy";
        public const string MerchantFeedMapping = "MerchantFeedMapping";
        public const string PriceAndAvailabilityDumping = "PriceAndAvailabilityDumping";
        public const string PartitionKey = "PartitionKey";
        public const string DecimalPrefix = "D_";
        public const string Percentage = "PERCENTAGE";
        public const string RowKey = "RowKey";
        public const string DumpingStatus = "DumpingStatus";
        public const string UserId = "0";
        public const string GoogleMapsOrder = "GoogleMapsOrder";
        public const string BookingStatus = "BookingStatus";
        public const string CancelledBookingStatus = "Cancelled";
        public const string IsNotifiedToGoogle = "IsNotifiedToGoogle";
        public const string MaxParallelThreadCount = "MaxParallelThreadCount";
        public const string APIIdsAvailabilityFromDB = "APIIdsAvailabilityFromDB";
        public const string AffiliateWiseServiceMinPrice = "AffiliateWiseServiceMinPrice";
        public const string GetCurrencyExchangeRates = "GetCurrencyExchangeRates";
        public const string IsCosmosInsertDeleteLogging = "IsCosmosInsertDeleteLogging";
        public const string WebAPIBaseUrl = "WebAPIBaseUrl";
        public const string CacheDeleteToken = "CacheDeleteToken";

        public const string IsRiskifiedEnabled = "IsRiskifiedEnabled";
        public const string IsRiskifiedEnabledWith3D = "IsRiskifiedEnabledWith3D";
        public const string IsRiskifiedTestingPhase = "IsRiskifiedTestingPhase";

        public const string IsReservation = "IsReservation";

        public const string TiqetsAsyncCallInterval = "TiqetsGetTicketCallInterval";

        public const string TiqetsTicketDelay = "TiqetsTicketDelay";

        public const string TiqetsRetryThreshold = "TiqetsRetryThreshold";
        public const string RezdyLabelDetails = "RezdyLabelDetails";
        public const string RezdyPaxMapping = "RezdyPaxMapping";
        public const string B2BUserKey = "B2BUserKey";
        public const string RezdyTotalRecordsLimit = "RezdyTotalRecordsLimit";
        public const string RezdyFixedRecordGetOneTime = "RezdyFixedRecordGetOneTime";

        public const string DefaultCurrencyISOCode = "GBP";
        public const string DefaultCountryISOCode = "IN";
        public const string Cancelled = "CANCELLED";

        public const string WebsitesToClearActivityCache = "WebsitesToClearActivityCache";
        public const string AppSettingsCacheDeleteToken = "CacheDeleteToken";

        /// <summary>
        /// Single Activity : "cache/clearkey?key={0}&cachedeletetoken={1}"
        /// Multiple Activities : "cache/clearkey?key={1}&cachedeletetoken={1}"
        /// 0 Example : Single "activity_5850" or Multiple "activity_5850,activity_5851"
        /// 1 CacheDeleteToken from sites webconfig file
        /// </summary>
        public const string WebsitesCacheKeyActivity = "cache/clearkey?key={0}&cachedeletetoken={1}";

        public const string WebsitesCacheKeyActivityAll = "cache/clearkey?isclearallactivity={0}&cachedeletetoken={1}";

        //Error Messages Start
        public const string NotFound = " not found in cache and db.";
        public const string OnePaxRequired = "At least one pax required that can be booked independently";
        public const string AgeGroupNotMatched = "Age Group not matched.";
        public const string ProductOptionNotExist = "Product Options not Exist.";
        public const string ActivityOptionMapping = "Activity option mapping not found for";
        public const string ProductOptionPriceNotExist = "Product Options price or availability not Exist.";
        public const string APIActivityNot = "No data return from";
        public const string APIActivityOptionsNot = "No Options return from";
        public const string FareHarbourAPI = " FareHarbour API";
        public const string AOTAPI = " AOT API";
        public const string BokunAPI = " Bokun API";
        public const string GlobalTixAPI = " GlobalTix API";
        public const string GoldenTourAPI = " GoldenTour API";
        public const string GrayLineIcelandAPI = " GrayLineIceland API";
        public const string HotelBedsAPI = " HotelBeds API";
        public const string MoulinRougeAPI = " MoulinRouge API";
        public const string PRIOAPI = " PRIO API";
        public const string PRIOHUBAPI = " PRIO HUB API";
        public const string RedeamAPI = " Redeam API";
        public const string RezdyAPI = " Rezdy API";
        public const string TiqetsAPI = " Tiqets API";
        public const string VentrataAPI = " Ventrata API";
        public const string DefaultRangeExceed = "Default Range Exceeded!! CheckAvailability CheckIndDate & CheckOutDate Default Range is";
        public const string DefaultRangeExceedDays = " Days, Please try again";
        public const string DataNotInTableStorage = "Data not exist in table storage";
        public const string ExceptionIsAPIBooking = "Exception while isAPIBooking check";
        public const string InvalidPayment = "Invalid Payment option for Alternative payment.";
        public const string NoSelectedProductOrAffiliate = "Data not Exists for Products or Affiiate";
        public const string TourCMSPaxMapping = "TourCMSPaxMapping";
        public const string GlobalTixPaxMapping = "GlobalTixV3PaxMapping";
        public const string VentrataPaxMapping = "VentrataPaxMapping";
        public const string NewCitySightSeeingAPI = "NewCitySightSeeing API";
        //Error Messages End
        public const string ElasticSearchAPIEndPoint = "ElasticSearchAPIEndPoint";
        public const string ElasticDestinationEndPoint = "ElasticDestinationEndPoint";
        public const string ElasticProductsEndPoint = "ElasticProductsEndPoint";
        public const string ElasticAttractionsEndPoint = "ElasticAttractionsEndPoint";
        public const string ElasticAffiliateEndPoint = "ElasticAffiliatesEndPoint";

        public const int twelveHours = 720;
        public const string authorizationError = "invalid_grant";
        public const string b2CAuthentication = "B2CAuthentication";


        //Isango Common Code Formats:
        public const string IsangoBARCODE = "BAR_CODE";
        public const string IsangoQRCODE = "QR_CODE";
        public const string IsangoLINK = "LINK";

        //PrioHub Code Formats:
        public const string PrioHub_BARCODE = "BAR_CODE";
        public const string PrioHub_BARCODEE128 = "BAR_CODE_E128";
        public const string PrioHub_BARCODEC128 = "BAR_CODE_C128";
        public const string PrioHub_BARCODEC39 = "BAR_CODE_C39";
        public const string PrioHub_AZTEC = "AZTEC";
        public const string PrioHub_PDF = "PDF";
        public const string PrioHub_PDF417 = "PDF417";
        public const string PrioHub_QRCODE = "QR_CODE";
        public const string PrioHub_QRCODELINK = "QR_CODE_LINK";
        public const string PrioHub_LINK = "LINK";
        public const string PrioHub_IMAGE = "IMAGE";

        //TourCMS Code Formats:
        public const string TourCMS_BARCODE = "CODE_128";
        public const string TourCMS_BAR = "bar";


        //Rayna Code Formats:
        public const string API_Link = "Link";
    }
}