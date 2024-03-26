namespace ServiceAdapters.GoldenTours.Constants
{
    public class UriConstants
    {
        public const string ProductDetails = "/productdetails.aspx?productid={0}&key={1}&currencycode={2}&languageid={3}";
        public const string Availability = "/availability.aspx?productid={0}&key={1}&day={2}&month={3}&year={4}";
        public const string GetProductDates = "/getProductDates.aspx?productid={0}&key={1}&status={2}&fromdt={3}&todt={4}";
        public const string GetBookingDates = "/getBookingDates.aspx?productid={0}&key={1}&status={2}&fromdt={3}&todt={4}";
        public const string PickupPoints = "/blockpickuppoint.aspx?productid={0}&key={1}";
        public const string Booking = "/booking.aspx";
    }
}