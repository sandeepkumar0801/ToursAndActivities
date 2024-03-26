using System.Net.Http.Headers;

namespace ServiceAdapters.PrioHub.Constants
{
    public sealed class Constant
    {
        public const string SaveInStorage = "SaveInStorage";
        public const string SaveInStorageValue = "1";
        public const string DateFormat = "yyyy-MM-dd";
        public const string Child = "CHILD";
        public const string Infant = "INFANT";
        public const string Senior = "SENIOR";
        public const string Adult = "ADULT";
        public const string Family = "FAMILY";
        public const string Student = "STUDENT";
        public const string Youth = "YOUTH";
        public const string BasePrice = "BasePrice";
        public const string CostPrice = "CostPrice";
        public const string GatePrice = "GatePrice";
        public const string NewPrioBaseAddress = "PrioHubBaseAddress";
        public const string PrioHubBaseAddressOnlyPrioProducts = "PrioHubBaseAddressOnlyPrioProducts";

        public const string NewPrioApiUser = "PrioHubApiUser";
        public const string NewPrioApiKey = "PrioHubApiKey";
        public const string NewPrioApiDistributorId = "PrioHubApiDistributorId";

        public const string NewPrioApiUserPrioOnly = "PrioHubApiUserOnlyPrioProducts";
        public const string NewPrioApiKeyPrioOnly = "PrioHubApiKeyOnlyPrioProducts";
        public const string NewPrioApiDistributorIdPrioOnly = "PrioHubApiDistributorIdOnlyPrioProducts";


        public const string NewPrioApiScopeProducts = "PrioHubApiScopeProducts";
        public const string NewPrioApiScopeReservation = "PrioHubApiScopeReservation";
        public const string NewPrioApiScopeBooking = "PrioHubApiScopeBooking";
        public const string NewPrio = "PrioHub";
        public const string Products = "products";
        public const string Orders = "orders";
        public const string Reservations = "reservations";
        public const string Booking = "orders";
        public const string Availability = "availability";
        public const string Routes = "/routes/";
        public const string Oauth2 = "oauth2/token";
        public const string ProductAvailability = "ProductAvailability";
     
        public const string RequestPrepare = "Request Prepare, it should change";
        public const string HeaderContentType = "Content-Type";
        public const string HeaderApplicationJson = "application/json";
        public const string HeaderApiKey = "Authorization";
        public const string Accept = "Accept";
        public const string Authorization = "Authorization";
        public const string Bearer = "Bearer ";
        
        public const string App_Json = "application/json";
        public const string Content_type = "Content-type";

        public const string ErrorWhileAPIHit = "ErrorWhileAPIHit";
        public static MediaTypeHeaderValue HttpHeader_ContentType_JSON = MediaTypeHeaderValue.Parse("application/json");

        public const string Reservation = "Reservation";
        public const string BookReservation = "BookReservation";
        public const string Cancellation = "cancelBooking";
        public const string Exception1 = "ConvertAvailablityResult :- PrioHub Availabilities not found";
        public const string DateTimeWithSecondFormat = "yyyy-MM-dd'T'HH:mm:sszzz";
    }
}