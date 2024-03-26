namespace ServiceAdapters.PrioHub.PrioHub.Entities
{
    public enum ProductRelationType
    {
        RELATED = 0,
        //Similar products which could be shown.
        //Other museums in the region. (Cross-product)
        UPGRADE = 1,
        //This cluster consists of one or more products which can act as an upgrade. 
        OPTION = 2,
        //Several flavors of a product. (Inter-product)
        SEATING = 3,
        //To be announced
        DESTINATION = 4,
        //To be announced
        OTHER = 5,
        // Non-defined.
    }
}