namespace ServiceAdapters.Bokun.Bokun.Entities
{
    public static class UriConstants
    {
        public const string CheckAvailabilities = "/activity.json/{0}/availabilities?start={1}&end={2}&lang=EN&currency={3}&includeSoldOut=false";
        public const string CheckoutOptions = "/checkout.json/options/booking-request";
        public const string SubmitCheckout = "/checkout.json/submit?currency={0}";
        public const string EditBooking = "/booking.json/edit";
        public const string GetActivityBookingDetails = "/booking.json/activity-booking/{0}";
        public const string GetConfirmedBooking = "/booking.json/booking/{0}";
        public const string CanceBooking = "/booking.json/cancel-booking/{0}";
        public const string GetActivity = "/activity.json/{0}";
        public const string GetPickupPlaces = "/activity.json/{0}/pickup-places";
    }
}