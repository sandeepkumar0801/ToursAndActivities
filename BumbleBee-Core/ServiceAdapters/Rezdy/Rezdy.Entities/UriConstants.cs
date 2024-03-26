namespace ServiceAdapters.Rezdy.Rezdy.Entities
{
    public static class UriConstants
    {
        public const string Products = "v1/products/{0}";
        public const string AllProducts = "v1/products/marketplace?limit={0}&&offset={1}";
        public const string Availability = "v1/availability?productCode={0}&startTimeLocal={1}&&endTimeLocal={2}&&limit={3}&&offset={4}";
        public const string AllProductsForSupplierAlias = "v1/products/marketplace?supplierAlias={0}";
        public const string AllProductsForSupplierId = "v1/products/marketplace?supplierId={0}";
        public const string CreateBooking = "v1/bookings";
        public const string CancelBooking = "v1/bookings/{0}";
        public const string PickUpDetails = "v1/pickups/{0}";
    }
}