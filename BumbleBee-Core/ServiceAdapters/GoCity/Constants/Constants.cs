using System.Net.Http.Headers;

namespace ServiceAdapters.GoCity.Constants
{
    public sealed class Constant
    {
        public const string SaveInStorage = "SaveInStorage";
        public const string SaveInStorageValue = "1";
        public const string GoCityBaseAddress = "GoCityBaseAddress";
        public const string GoCityApiUserName = "GoCityApiUserName";
        public const string GoCityApiPassword = "GoCityApiPassword";
        public const string Products = "rest/secure/getProductsForSale";
        public const string Booking = "rest/commerce/v1/trade/order";
        public const string Cancellation = "rest/commerce/v1/trade/order/cancel";
        public const string RequestPrepare = "Request Prepare, it should change";
        public const string HeaderContentType = "Content-Type";
        public const string HeaderApplicationJson = "application/json";
        public const string HeaderApiKey = "Authorization";
        public const string GoCity = "GoCity";
        public const string ErrorWhileAPIHit = "ErrorWhileAPIHit";
        public static MediaTypeHeaderValue HttpHeader_ContentType_JSON = MediaTypeHeaderValue.Parse("application/json");
       
      
    }
}