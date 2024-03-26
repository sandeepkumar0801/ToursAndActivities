using Isango.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace ServiceAdapters.GlobalTixV3.Constants
{
    public sealed class Constant
    {
        public const string SaveInStorage = "SaveInStorage";
        public const string SaveInStorageValue = "1";
        public const string APINameGlobalTix = "GlobalTixV3";

        //----- Configuration parameters retrieved from app.config
        public const string CfgParam_GlobalTixBaseUrl = "GlobalTixBaseUrl";

        // For Non Thailand products
        public const string CfgParam_GTAuthuser = "GTAuthuser";

        public const string CfgParam_GTAuthpassword = "GTAuthpassword";

        //For Thailand products
        public const string CfgParam_GTAuthuser_ForThailand = "GTAuthuserForThailand";

        public const string CfgParam_GTAuthpassword_ForThailand = "GTAuthpasswordForThailand";

        //----- Constant literals
        public const string Const_GTSupplierName = "GlobalTix Singapore";

        //----- Http headers and values
        public const string HttpHeader_AcceptVersion = "Accept-Version";

        public const string HttpHeader_AcceptVersion_Val = "1.0";
        public const string HttpHeader_Authorization = "Authorization";
        public static MediaTypeHeaderValue HttpHeader_ContentType_JSON = MediaTypeHeaderValue.Parse("application/json");

        //----- Http GET services query parameters
        public const string QueryParam_AttractionId = "attraction_id";

        public const string QueryParam_CityId = "cityId";
        public const string QueryParam_CountryId = "countryId";
        public const string QueryParam_DateFrom = "dateFrom";
        public const string QueryParam_DateTo = "dateTo";
        public const string QueryParam_ProductId = "productId";
        public const string QueryParam_Id = "id";
        public const string QueryParam_Page = "page";
        public const string QueryParam_RefernceNumber = "reference_number";
        public const string QueryParam_ResellerPackageOnly = "resellerPackageOnly";
        public const bool QueryParam_ResellerPackageOnly_Val = false;

        //----- Supplier specific to isango framework mapping
        public static IDictionary<int, ActivityCategoryType> Mapper_CategoryType = new Dictionary<int, ActivityCategoryType>
        {
            {12, ActivityCategoryType.Themeparks}
        };

        public static IDictionary<string, PassengerType> Mapper_PassengerType = new Dictionary<string, PassengerType>
        {
            {"ADULT", PassengerType.Adult},
            {"CHILD", PassengerType.Child},
            {"SENIOR_CITIZEN", PassengerType.Senior },
            {"PER PAX", PassengerType.Adult },
            {"PAX", PassengerType.Adult },
            {"PER PAC", PassengerType.Adult },
            {"SENIOR", PassengerType.Senior }
        };

        //----- Service URLs relative to base URL
        public const string URL_Auth = "auth/login";

        public const string URL_ActivityList = "attraction/list";
        public const string URL_ActivityInfo = "attraction/get";
        public const string URL_ActivityTicketType = "ticketType/get";
        public const string URL_ActivityTicketTypes = "ticketType/list";
        public const string URL_CancelByTicket = "ticket/revoke";
        public const string URL_CancelByBooking = "transaction/revoke";
        public const string URL_CityList = "city/getAllCities";
        public const string URL_Categories = "merchantCategory/getAllCategories";
        public const string URL_CountryList = "country/getAllCountries";
        public const string URL_CreateBooking = "transaction/create";
        public const string URL_Reservation = "booking/reserve";
        public const string URL_FinalBooking = "booking/confirm";
        public const string URL_Image = "image";
        public const string URL_PackageInfo = "package/get";
        public const string URL_PackageList = "package/list";
        public const string URL_ProductList = "product/list";
        public const string URL_ProductInfo = "product/info";
        public const string URL_PackageOptions = "package/options";
        public const string URL_ProductChanges = "product/changes";
        public const string URL_ProductOption = "product/options";
        public const string URL_CheckEventAvailability = "ticketType/checkEventAvailability";


        // Image related constants
        public const string Image_Suffix_Banner = "_banner";

        public const string Image_Suffix_Thumb = "_thumb";

        // Booking status related constants
        public const string Book_Status_Valid = "VALID";

        // Package Prefix Character
        //public const char Package_Prefix = 'P';

        public const string AppSettings_Suffix_Notify_Isango = "_IsSendNotificationToIsango";
        public const string AppSettings_Suffix_Notify_Isango_Email = "_NotificationEmailAddressIsango";
        public const string Flag_True_Value = "1";

        public const string QRCode_Type_Link = "LINK";

        public const string FREETEXT = "FREETEXT";
        public const string OPTION = "OPTION";

        public const string QUESTION = "QUESTION";
        public const string PICKUP = "PICKUP";

        public const string CountryId = "Country_Id";
        public const string CityId = "City_Id";
        public const string CountryName = "Country_Name";
        public const string CityName = "City_Name";
        public const string CategoryId = "Category_Id";
        public const string CategoryName = "Category_Name";

        //variables for ProductInfoTable
        public const string Country = "Country";
        public const string OriginalPrice = "Original_Price";
        public const string BlockeDate = "Blocke_Date";
        public const string FromPrice = "From_Price";
        public const string City = "City";
        public const string Description = "Description";
        public const string Image = "Image";
        public const string Currency = "Currency";
        public const string Id = "Id";
        public const string IsGTRecommend = "Is_GTRecommend";
        public const string IsOpenDated = "Is_OpenDated";
        public const string IsOwnContracted = "Is_OwnContracted";
        public const string IsFavorited = "Is_Favorited";
        public const string IsBestSeller = "Is_BestSeller";
        public const string FromReseller = "From_Reseller";
        public const string Name = "Name";
        public const string LastUpdated = "LastUpdated";
        public const string IsInstantConfirmation = "is_Instant_Confirmation";
        public const string Location = "Location";
        public const string Category = "Category";


        public const string TermsAndConditions = "TermsAndConditions";
        public const string PublishStart = "PublishStart";
        public const string publishEnd = "PublishEnd";
        public const string redeemStart = "RedeemStart";
        public const string redeemEnd = "RedeemEnd";
        public const string ticketValidity = "Ticket_Validity";
        public const string ticketFormat = "Ticket_Format";
        public const string definedDuration = "Defined_Duration";
        public const string sourceName = "SourceName";
        public const string sourceTitle = "SourceTitle";
        public const string isAdditionalBookingInfo = "IsAdditionalBookingInfo";
        public const string keywords = "Keywords";



        public const string PackagetypeparentId = "PackagetypeparentId";
        public const string PackageType_Id = "PackageType_Id";
        public const string PackageType_Sku = "PackageType_Sku";
        public const string PackageType_Name = "PackageType_Name";
        public const string PackageType_NettPrice = "PackageType_NettPrice";
        public const string PackageType_SettlementRate = "PackageType_SettlementRate";
        public const string PackageType_OriginalPrice = "PackageType_OriginalPrice";
        public const string PackageType_IssuanceLimit = "PackageType_IssuanceLimit";






        public const string uspinsGlobalTixCountryCity = "usp_ins_GlobalTix_CountryCity";
        public const string uspgetGlobalTixCountryCity = "usp_get_GlobalTix_CountryCity";

        public const string uspinsGlobalTixCategoriesV3 = "usp_ins_GlobalTix_CategoriesV3";
        public const string uspinsGlobalTixProductChangesV3 = "usp_ins_GlobalTix_ProductChangesV3";
        public const string uspinsGlobalTixPackageOptionsV3 = "usp_ins_GlobalTix_PackageOptionsV3";
        public const string usp_ins_GlobalTix_PackageTypeIdV3 = "usp_ins_GlobalTix_PackageTypeIdV3";

        public const string CancellationTextNonRefundable = "Non-Refundable";

        public const string QueryParam_TicketTypeID = "ticketTypeID";

        public const string QueryParam_ReferenceNumber = "referenceNumber";
        public const string URL_BookingDetail = "booking/details";
    }
}
