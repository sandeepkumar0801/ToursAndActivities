using System.Net.Http.Headers;

namespace ServiceAdapters.NewCitySightSeeing.Constants
{
    public sealed class Constant
    {
        public const string SaveInStorage = "SaveInStorage";
        public const string SaveInStorageValue = "1";
        public const string NewCitySightSeeingBaseAddress = "NewCitySightSeeingBaseAddress";
        public const string NewCitySightSeeingApiKey = "NewCitySightSeeingApiKey";
        public const string Products = "Products";
        public const string ProductAvailability = "ProductAvailability";
        public const string CommonURL = "api/v1/integrations/CommonIntegration/";
        public const string RequestPrepare = "Request Prepare, it should change";
        public const string HeaderContentType = "Content-Type";
        public const string HeaderApplicationJson = "application/json";
        public const string HeaderApiKey = "Authorization";
        public const string NewCitySightSeeing = "NewCitySightSeeing";
        public const string ErrorWhileAPIHit = "ErrorWhileAPIHit";
        public static MediaTypeHeaderValue HttpHeader_ContentType_JSON = MediaTypeHeaderValue.Parse("application/json");

        public const string Reservation = "Reservation";
        public const string BookReservation = "BookReservation";
        public const string Cancellation = "cancelBooking";
    }
}