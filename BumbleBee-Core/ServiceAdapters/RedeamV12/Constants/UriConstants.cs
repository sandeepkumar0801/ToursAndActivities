namespace ServiceAdapters.RedeamV12.Constants
{
#pragma warning disable S1118 // Utility classes should not have public constructors

    public class UriConstants
#pragma warning restore S1118 // Utility classes should not have public constructors
    {
        public const string GetAvailabilities = "/suppliers/{0}/products/{1}/availabilities?start={2}&end={3}";
        public const string GetAvailability = "/suppliers/{0}/products/{1}/availability?at={2}&qty={3}";

        public const string GetRates = "/suppliers/{0}/products/{1}/rates";
        public const string GetRate = "/suppliers/{0}/products/{1}/rates/{2}";

        public const string CreateBooking = "/bookings";
        public const string CancelBooking = "/bookings/cancel/{0}";

        public const string CreateHold = "/holds";
        public const string DeleteHold = "/holds/{0}";

        public const string GetAllSuppliers = "/suppliers";
        public const string GetAllProducts = "/suppliers/{0}/products";

        
    }
}