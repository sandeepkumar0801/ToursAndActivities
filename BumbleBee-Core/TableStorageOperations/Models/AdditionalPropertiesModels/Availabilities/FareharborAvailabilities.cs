namespace TableStorageOperations.Models.AdditionalPropertiesModels.Availabilities
{
    public class FareharborAvailabilities : BaseAvailabilitiesEntity
    {
        public string UserKey { get; set; }
        public string AvailToken { get; set; }
        public string CustomerTypePriceIds { get; set; }
        public string SupplierName { get; set; }
    }
}