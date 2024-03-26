namespace ServiceAdapters.PrioHub.PrioHub.Entities
{
    public enum ProductCapacityType
    {
        OWN = 0,
        // This product has his own capacity.
        SHARED = 1,
        //This product does not have it's own capacity, instead it uses shared capacity from other products.
        COMBINED = 2,
        // This product has his own capacity combined with the capacity of other products.
        NOT_SET = 3,
        //This product does not have capacity.
    }
}