namespace ServiceAdapters.PrioHub.PrioHub.Entities
{
    public enum ProductTypePriceType
    {
        INDIVIDUAL = 0,
        //Depending on the booking quantity, the price increases
        GROUP = 1,
        //The price for this product type is fixed regardless of
        //how many are booked.
    }
}