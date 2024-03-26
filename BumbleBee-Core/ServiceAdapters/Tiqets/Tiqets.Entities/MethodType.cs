namespace ServiceAdapters.Tiqets.Tiqets.Entities
{
    public enum MethodType
    {
        Undefined = 0,
        CheckoutInformation = 1,
        AvailableDays = 2,
        AvailableTimeSlots = 3,
        Variant = 4,
        CreateOrder = 5,
        ConfirmOrder = 6,
        GetTicket = 7,
        ProductDetails = 8,
        BulkAvailability = 9,
        GetOrderInfo = 10,
        CancelOrder = 11,
        ProductFilter = 12,
    }
}