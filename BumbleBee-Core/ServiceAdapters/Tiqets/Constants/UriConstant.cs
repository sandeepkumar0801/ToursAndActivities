namespace ServiceAdapters.Tiqets.Constants
{
    public static class UriConstant
    {
        public const string Products = "products/";
        public const string CheckoutInformation = "/checkout_information?lang=";
        public const string AvailableDays = "/available_days?lang=";
        public const string AvailableTimeSlots = "/timeslots?lang=";
        public const string Day = "&day=";
        public const string Variants = "/variants?lang=";
        public const string TimeSlot = "&timeslot=";
        public const string Language = "?lang=";
        public const string BulkAvailability = "/bulk_availability";
        public const string BulkAvailabilityVariants = "/availability";
        public const string BulkAvailabilityWithEndDate = "?end_date=";
        public const string BulkVariantsAvailabilityWithEndDate = "&end_date=";
        public const string BulkAvailabilityWithStartDate = "&start_date=";
        public const string CreateOrder = "orders?lang=";
        public const string ConfirmOrder = "orders/";
        public const string GetOrderInfo = "orders/";
        public const string CancelOrder = "orders/";
        public const string BookingLanguage = "?lang=";
        public const string Tickets = "/tickets";
        public const string ProductsFilter = "products";
    }
}