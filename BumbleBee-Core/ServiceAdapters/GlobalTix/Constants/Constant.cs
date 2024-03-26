using Isango.Entities.Enums;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace ServiceAdapters.GlobalTix.Constants
{
    public sealed class Constant
    {
        public const string SaveInStorage = "SaveInStorage";
        public const string SaveInStorageValue = "1";
        public const string APINameGlobalTix = "GlobalTix";

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
            {"PER PAX", PassengerType.Adult }
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
        public const string URL_CountryList = "country/getAllListingCountry";
        public const string URL_CreateBooking = "transaction/create";
        public const string URL_Image = "image";
        public const string URL_PackageInfo = "package/get";
        public const string URL_PackageList = "package/list";

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
        public const string CategoryId = "Category_Id";
        public const string CategoryName = "Category_Name";
        public const string CountryName = "Country_Name";
        public const string CityName = "City_Name";
        public const string uspinsGlobalTixCountryCity = "usp_ins_GlobalTix_CountryCity";
        public const string uspgetGlobalTixCountryCity = "usp_get_GlobalTix_CountryCity";

        public const string uspinsGlobalTixCountryCityV3 = "usp_ins_GlobalTix_CountryCityV3";
        public const string uspinsGlobalTixProductInfoV3 = "usp_ins_GlobalTix_ProductListV3";

        public const string uspgetGlobalTixCountryCityV3 = "usp_get_GlobalTixV3_CountryCity";

        public const string CancellationTextNonRefundable = "Non-Refundable";
        public const string Update_GlobalV3TixData = "usp_update_GlobalV3TixData";

    }
}